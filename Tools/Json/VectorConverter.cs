using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DVG.SkyPirates.Shared.Tools.Json
{
    public class VectorConverter<T, V> : JsonConverter<V>
        where V : unmanaged
        where T : unmanaged
    {
        private readonly int _count = (int)typeof(V).GetProperty("Count").GetValue(new V());

        private readonly Utf8JsonWriter _jsonWriter;
        private readonly ArrayBufferWriter<byte> _bufferWriter = new();
        private readonly ArrayBufferWriter<byte> _bufferCache = new();

        private readonly byte[] _arrayStart = Encoding.UTF8.GetBytes("[");
        private readonly byte[] _arrayEnd = Encoding.UTF8.GetBytes("]");
        private readonly byte[] _comma = Encoding.UTF8.GetBytes(",");

        public VectorConverter()
        {
            _jsonWriter = new(_bufferWriter, new() { Indented = false });
        }

        public override V Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var vector = new V();
            var span = MemoryMarshal.CreateSpan(ref Unsafe.As<V, T>(ref vector), _count);

            for (int i = 0; i < _count; i++)
            {
                reader.Read();
                span[i] = JsonSerializer.Deserialize<T>(ref reader, options);
            }
            reader.Read();
            return vector;
        }

        public override V ReadAsPropertyName(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var vector = new V();
            var span = MemoryMarshal.CreateSpan(ref Unsafe.As<V, T>(ref vector), _count);

            _bufferWriter.Clear();

            _bufferWriter.Write(_arrayStart);
            int written = reader.CopyString(_bufferWriter.GetSpan(reader.ValueSpan.Length));
            _bufferWriter.Advance(written);
            _bufferWriter.Write(_arrayEnd);

            var innerReader = new Utf8JsonReader(_bufferWriter.WrittenSpan);
            innerReader.Read();
            for (int i = 0; i < _count; i++)
            {
                innerReader.Read();
                span[i] = JsonSerializer.Deserialize<T>(ref innerReader, options);
            }
            return vector;
        }

        public override void Write(Utf8JsonWriter writer, V vector, JsonSerializerOptions options)
        {
            var span = MemoryMarshal.CreateSpan(ref Unsafe.As<V, T>(ref vector), _count);
            _bufferCache.Clear();
            _bufferCache.Write(_arrayStart);
            for (int i = 0; i < _count; i++)
            {
                _jsonWriter.Reset();
                _bufferWriter.Clear();
                JsonSerializer.Serialize(_jsonWriter, span[i], options);
                _bufferCache.Write(_bufferWriter.WrittenSpan);
                if (i != _count - 1)
                    _bufferCache.Write(_comma);
            }
            _bufferCache.Write(_arrayEnd);
            var res = Encoding.UTF8.GetString(_bufferCache.WrittenSpan);
            writer.WriteRawValue(_bufferCache.WrittenSpan);
        }

        public override void WriteAsPropertyName(Utf8JsonWriter writer, V vector, JsonSerializerOptions options)
        {
            var span = MemoryMarshal.CreateSpan(ref Unsafe.As<V, T>(ref vector), _count);
            _bufferCache.Clear();
            for (int i = 0; i < _count; i++)
            {
                _jsonWriter.Reset();
                _bufferWriter.Clear();
                JsonSerializer.Serialize(_jsonWriter, span[i], options);
                _bufferCache.Write(_bufferWriter.WrittenSpan);
                if (i != _count - 1)
                    _bufferCache.Write(_comma);
            }
            writer.WritePropertyName(_bufferCache.WrittenSpan);
        }
    }
}
