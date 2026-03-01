using DVG.SkyPirates.Shared.Data;
using DVG.SkyPirates.Shared.IServices;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;
using DVG.SkyPirates.Shared.Tools.Json;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace DVG.SkyPirates.Shared.Services
{
    public class HashSumService : IHashSumService, ITickableExecutor
    {
        private readonly IHistorySystem _historySystem;
        private readonly ArrayBufferWriter<byte> _bufferWriter;
        private readonly Utf8JsonWriter _fastWriter;
        private readonly Utf8JsonWriter _prettyWriter;
        private readonly HashAlgorithm _hashing;
        private readonly StringBuilder _sBuilder;
        private readonly JsonSerializerOptions _serializerOptions;

        private readonly byte[] _hashResult;
        private string? _sHash;

        private readonly Dictionary<int, (string hash, WorldData data, int version)> _hashHistory = new();

        public HashSumService(IHistorySystem historySystem)
        {
            _historySystem = historySystem;

            _serializerOptions = new(SerializationUTF8.Options)
            {
                WriteIndented = false,
            };
            _bufferWriter = new ArrayBufferWriter<byte>();
            _fastWriter = new(_bufferWriter, new JsonWriterOptions() { Indented = false, SkipValidation = true });
            _prettyWriter = new(_bufferWriter, new JsonWriterOptions() { Indented = true, SkipValidation = true });
            _hashing = SHA512.Create();
            _sBuilder = new StringBuilder();
            _hashResult = new byte[_hashing.HashSize / 8];
        }

        public (string hash, WorldData snapshot, int version) GetSnapshot(int tick)
        {
            _hashHistory.TryGetValue(tick, out var entry);
            return entry;
        }

        public Dictionary<int, (string hash, WorldData snapshot, int version)> GetSnapshots()
        {
            return new(_hashHistory);
        }

        public void Tick(int tick)
        {
            var worldData = _historySystem.GetSnapshot(tick);
            using var document = JsonSerializer.SerializeToDocument(worldData, _serializerOptions);
            _fastWriter.Reset();
            _bufferWriter.Clear();
            WriteOrdered(_fastWriter, document.RootElement);
            _fastWriter.Flush();
            if (!_hashing.TryComputeHash(_bufferWriter.WrittenSpan, _hashResult, out _))
                Debug.Assert(false, "Failed to get HashSum");

            _sBuilder.Clear();
            for (int i = 0; i < _hashResult.Length; i++)
                _sBuilder.Append(_hashResult[i].ToString("x2"));

            _sHash = _sBuilder.ToString();

            if (!_hashHistory.TryGetValue(tick, out var entry))
                _hashHistory[tick] = entry = new();
            entry.hash = _sHash;
            entry.data = worldData;
            entry.version++;
            _hashHistory[tick] = entry;
        }

        private void WriteOrdered(Utf8JsonWriter jsonWriter, JsonElement element)
        {
            if (element.ValueKind is JsonValueKind.Object)
            {
                var count = element.GetPropertyCount();
                var i = count;
                var array = ArrayPool<JsonProperty>.Shared.Rent(count);
                foreach (var item in element.EnumerateObject())
                    array[--i] = item;
                Array.Sort(array, 0, count, JsonPropertyComparer.Default);

                jsonWriter.WriteStartObject();
                for (; i < count; i++)
                {
                    jsonWriter.WritePropertyName(JsonMarshal.GetRawUtf8PropertyName(array[i]));
                    WriteOrdered(jsonWriter, array[i].Value);
                }
                jsonWriter.WriteEndObject();
                ArrayPool<JsonProperty>.Shared.Return(array);
            }

            else if (element.ValueKind is JsonValueKind.Array)
            {
                jsonWriter.WriteStartArray();
                foreach (var item in element.EnumerateArray())
                    WriteOrdered(jsonWriter, item);
                jsonWriter.WriteEndArray();
            }

            else
            {
                element.WriteTo(jsonWriter);
            }
        }


        private class JsonPropertyComparer : IComparer<JsonProperty>
        {
            public static JsonPropertyComparer Default { get; } = new();

            public int Compare(JsonProperty x, JsonProperty y)
            {
                var xRaw = JsonMarshal.GetRawUtf8PropertyName(x);
                var yRaw = JsonMarshal.GetRawUtf8PropertyName(y);
                return xRaw.SequenceCompareTo(yRaw);
            }
        }
    }
}
