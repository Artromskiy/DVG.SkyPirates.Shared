using System;
using System.Buffers;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DVG.SkyPirates.Shared.Tools.Json
{
    public class VectorConverter<T, V> : JsonConverter<V>
    {
        private object[] _values;
        private Utf8JsonWriter _jsonWriter;
        private readonly ArrayBufferWriter<byte> _bufferWriter = new();
        private readonly int _count = (int)typeof(V).GetField("Count").GetRawConstantValue();
        private readonly PropertyInfo _indexer = Array.Find(typeof(V).GetProperties(), p =>
        {
            var parameters = p.GetIndexParameters();
            return parameters.Length == 1 && parameters[0].ParameterType == typeof(int);
        });
        private readonly byte[] _arrayStart = Encoding.UTF8.GetBytes("[");
        private readonly byte[] _arrayEnd = Encoding.UTF8.GetBytes("]");

        private object[] Values => _values ??= new object[_count];
        private Utf8JsonWriter JsonWriter => _jsonWriter ??= new(_bufferWriter, new() { Indented = false });

        public override V Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            Array.Clear(Values, 0, _count);

            for (int i = 0; i < _count; i++)
            {
                reader.Read();
                Values[i] = JsonSerializer.Deserialize<T>(ref reader, options);
            }
            reader.Read();
            return (V)Activator.CreateInstance(typeToConvert, Values);
        }

        public override V ReadAsPropertyName(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            Array.Clear(Values, 0, _count);
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
                Values[i] = JsonSerializer.Deserialize<T>(ref innerReader, options);
            }
            return (V)Activator.CreateInstance(typeToConvert, Values);
        }

        public override void Write(Utf8JsonWriter writer, V value, JsonSerializerOptions options)
        {
            _bufferWriter.Clear();
            JsonWriter.Reset();
            object[] index = new object[1];
            for (int i = 0; i < _count; i++)
            {
                index[0] = i;
                JsonSerializer.Serialize(JsonWriter, _indexer.GetValue(value, index), options);
            }
            writer.WriteRawValue(_bufferWriter.WrittenSpan);
        }

        public override void WriteAsPropertyName(Utf8JsonWriter writer, V value, JsonSerializerOptions options)
        {
            JsonWriter.Reset();
            object[] index = new object[1];
            for (int i = 0; i < _count; i++)
            {
                index[0] = i;
                JsonSerializer.Serialize(JsonWriter, _indexer.GetValue(value, index), options);
            }
            writer.WritePropertyName(_bufferWriter.WrittenSpan);
        }
    }
}
