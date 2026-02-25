using System;
using System.Buffers;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DVG.SkyPirates.Shared.Tools.Json
{
    public class VectorConverter<T, V> : JsonConverter<V>
    {
        private readonly int Count = (int)typeof(V).GetField("Count").GetRawConstantValue();
        private readonly PropertyInfo Indexer = Array.Find(typeof(V).GetProperties(), p =>
        {
            var parameters = p.GetIndexParameters();
            return parameters.Length == 1 && parameters[0].ParameterType == typeof(int);
        });

        public override V Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            object[] values = Array.ConvertAll(
                JsonSerializer.Deserialize<T[]>(ref reader, options), e => (object)e!)!;

            return (V)Activator.CreateInstance(typeToConvert, values);
        }

        public override V ReadAsPropertyName(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            object[] values = Array.ConvertAll(reader.GetString()!.Split(","), e =>
                (object)JsonSerializer.Deserialize<T>(e, options)!);

            return (V)Activator.CreateInstance(typeToConvert, values);
        }

        public override void Write(Utf8JsonWriter writer, V value, JsonSerializerOptions options)
        {
            var bufferWriter = new ArrayBufferWriter<byte>();
            using var innerWriter = new Utf8JsonWriter(bufferWriter, new() { Indented = false });
            JsonSerializer.Serialize(innerWriter, GetRawValues(value), options);
            innerWriter.Flush();
            writer.WriteRawValue(bufferWriter.WrittenSpan);
        }

        public override void WriteAsPropertyName(Utf8JsonWriter writer, V value, JsonSerializerOptions options)
        {
            var values = GetValues(value, options);
            writer.WritePropertyName(string.Join(",", values));
        }

        private object[] GetRawValues(V value)
        {
            object[] values = new object[Count];
            object[] index = new object[1];
            for (int i = 0; i < Count; i++)
            {
                index[0] = i;
                values[i] = Indexer.GetValue(value, index);
            }
            return values;
        }

        private object[] GetValues(V value, JsonSerializerOptions options)
        {
            string[] values = new string[Count];
            object[] index = new object[1];
            for (int i = 0; i < Count; i++)
            {
                index[0] = i;
                values[i] = JsonSerializer.Serialize(Indexer.GetValue(value, index), options);
            }
            return values;
        }
    }
}
