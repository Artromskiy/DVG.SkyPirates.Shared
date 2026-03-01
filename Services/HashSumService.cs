using DVG.SkyPirates.Shared.IServices.TickableExecutors;
using DVG.SkyPirates.Shared.Tools.Json;
using System.Buffers;
using System.Diagnostics;
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
        private readonly JsonSerializerOptions _serializerOptions;

        private readonly byte[] _hashResult;
        private string? _sHash;

        public HashSumService(IHistorySystem historySystem)
        {
            _historySystem = historySystem;

            _serializerOptions = new(SerializationUTF8.Options)
            {
                WriteIndented = false,
            };
            _bufferWriter = new ArrayBufferWriter<byte>();
            _jsonWriter = new(_bufferWriter, new JsonWriterOptions() { Indented = true, SkipValidation = true });
            _hashing = SHA512.Create();
            _sBuilder = new StringBuilder();
            _hashResult = new byte[_hashing.HashSize / 8];
        }

        public void Tick(int tick)
        {
            int targetTick = Maths.Max(0, tick - Constants.ValidTicksCount);
            var worldData = _historySystem.GetSnapshot(targetTick);
            _jsonWriter.Reset();
            _bufferWriter.Clear();
            JsonSerializer.Serialize(_jsonWriter, worldData, _serializerOptions);
            if (!_hashing.TryComputeHash(_bufferWriter.WrittenSpan, _hashResult, out _))
                Debug.Assert(false, "Failed to get HashSum");

            _sBuilder.Clear();
            for (int i = 0; i < _hashResult.Length; i++)
                _sBuilder.Append(_hashResult[i].ToString("x2"));

            _sHash = _sBuilder.ToString();

            Debug.WriteLine($"Tick: {targetTick}. Hash: {_sHash}");
        }
    }
}
