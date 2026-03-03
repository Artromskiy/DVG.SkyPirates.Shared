using DVG.SkyPirates.Shared.Data;
using DVG.SkyPirates.Shared.IServices;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;
using DVG.SkyPirates.Shared.Tools.Json;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO.Hashing;
using System.Text.Json;

namespace DVG.SkyPirates.Shared.Services
{
    public class HashSumService : IHashSumService
    {
        private readonly IHistorySystem _historySystem;
        private readonly ArrayBufferWriter<byte> _bufferWriter;
        private readonly Utf8JsonWriter _jsonWriter;
        private readonly JsonSerializerOptions _serializerOptions;

        private readonly Dictionary<int, (string hash, WorldData data, int version)> _hashHistory = new();

        public HashSumService(IHistorySystem historySystem)
        {
            _historySystem = historySystem;

            _serializerOptions = new(SerializationUTF8.Options)
            {
                WriteIndented = false,
            };
            _bufferWriter = new ArrayBufferWriter<byte>();
            _jsonWriter = new(_bufferWriter, new JsonWriterOptions() { Indented = false, SkipValidation = true });
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
            _jsonWriter.Reset();
            _bufferWriter.Clear();
            //There are probably hashing algorithms that don't depend on order, xor or sum :)
            _jsonWriter.WriteOrdered(document.RootElement);
            _jsonWriter.Flush();

            var hash = XxHash32.HashToUInt32(_bufferWriter.WrittenSpan);

            if (!_hashHistory.TryGetValue(tick, out var entry))
                _hashHistory[tick] = entry = new();

            entry.hash = hash.ToString();
            entry.data = worldData;
            entry.version++;
            _hashHistory[tick] = entry;

            Console.WriteLine(hash);
        }
    }
}
