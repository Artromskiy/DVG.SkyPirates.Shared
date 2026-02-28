using DVG.SkyPirates.Shared.IServices.TickableExecutors;
using DVG.SkyPirates.Shared.Tools.Json;
using System;
using System.Buffers;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace DVG.SkyPirates.Shared.Services
{
    public class HashSumService : ITickableExecutor
    {
        private readonly IHistorySystem _historySystem;
        private readonly ArrayBufferWriter<byte> _bufferWriter;
        private readonly Utf8JsonWriter _jsonWriter;
        private readonly HashAlgorithm _hashing;
        private readonly StringBuilder _sBuilder;

        private readonly byte[] _hashResult;
        private string? _sHash;

        public HashSumService(IHistorySystem historySystem)
        {
            _historySystem = historySystem;

            _bufferWriter = new ArrayBufferWriter<byte>();
            _jsonWriter = new(_bufferWriter);
            _hashing = SHA512.Create();
            _sBuilder = new StringBuilder();
            _hashResult = new byte[_hashing.HashSize / 8];
        }

        public void Tick(int tick)
        {
            int targetTick = Maths.Max(0, tick - Constants.ValidTicksCount);
            var worldData = _historySystem.GetSnapshot(targetTick);
            using var document = JsonSerializer.SerializeToDocument(worldData, SerializationUTF8.Options);
            _jsonWriter.Reset();
            _bufferWriter.Clear();
            WriteOrdered(_jsonWriter, document.RootElement);
            _jsonWriter.Flush();
            if (!_hashing.TryComputeHash(_bufferWriter.WrittenSpan, _hashResult, out _))
                Debug.Assert(false, "Failed to get HashSum");

            _sBuilder.Clear();
            for (int i = 0; i < _hashResult.Length; i++)
                _sBuilder.Append(_hashResult[i].ToString("x2"));

            _sHash = _sBuilder.ToString();

            Debug.WriteLine($"Tick: {targetTick}. Hash: {_sHash}");
        }

        private void WriteOrdered(Utf8JsonWriter jsonWriter, JsonElement element)
        {
            if (element.ValueKind is JsonValueKind.Object)
            {
                jsonWriter.WriteStartObject();
                var properties = element.EnumerateObject().OrderBy(p => p.Name, StringComparer.Ordinal);
                foreach (var prop in properties)
                {
                    jsonWriter.WritePropertyName(prop.Name);
                    WriteOrdered(jsonWriter, prop.Value);
                }
                jsonWriter.WriteEndObject();
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
    }
}
