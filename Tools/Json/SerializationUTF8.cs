using System;
using System.Buffers;
using System.Text;
using System.Text.Json;

namespace DVG.SkyPirates.Shared.Tools.Json
{
    public static class SerializationUTF8
    {
        private static Utf8JsonWriter? _writer;
        private static readonly ArrayBufferWriter<byte> _buffer;
        private static readonly JsonSerializerOptions _options;
        static SerializationUTF8()
        {
            _buffer = new ArrayBufferWriter<byte>();
            _options = new(JsonSerializerOptions.Default);
            _options.IncludeFields = true;
            _options.IgnoreReadOnlyFields = false;
            _options.IgnoreReadOnlyProperties = false;
        }

        public static string Serialize<T>(T data)
        {
            _buffer.Clear();
            Serialize(data, _buffer);

            return Encoding.UTF8.GetString(_buffer.WrittenSpan);
        }

        public static void Serialize<T>(T data, IBufferWriter<byte> buffer)
        {
            _writer?.Reset(buffer);
            _writer ??= new Utf8JsonWriter(buffer);
            JsonSerializer.Serialize(_writer, data, _options);
        }

        public static T Deserialize<T>(string json)
        {
            _buffer.Clear();
            int count = Encoding.UTF8.GetByteCount(json);
            count = Encoding.UTF8.GetBytes(json, _buffer.GetSpan(count));
            _buffer.Advance(count);

            return Deserialize<T>(_buffer.WrittenMemory);
        }

        public static T Deserialize<T>(ReadOnlyMemory<byte> data)
        {
            var reader = new Utf8JsonReader(data.Span);
            return JsonSerializer.Deserialize<T>(ref reader, _options);
        }
    }
}
