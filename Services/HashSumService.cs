using DVG.SkyPirates.Shared.Data;
using DVG.SkyPirates.Shared.IServices;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;
using DVG.SkyPirates.Shared.Tools.Json;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace DVG.SkyPirates.Shared.Services
{
    public class HashSumService : IHashSumService
    {
        private readonly IHistorySystem _historySystem;
        private readonly ArrayBufferWriter<byte> _bufferWriter;
        private readonly Utf8JsonWriter _jsonWriter;
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
            _jsonWriter = new(_bufferWriter, new JsonWriterOptions() { Indented = false, SkipValidation = true });
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
            _jsonWriter.Reset();
            _bufferWriter.Clear();
            _jsonWriter.WriteOrdered(document.RootElement);
            _jsonWriter.Flush();
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

            Console.WriteLine(_sHash);
        }
    }
}
