using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DVG.SkyPirates.Shared.Tools.Json
{
    public class FuncConverter<T> : SimpleConverter<T>
    {
        private readonly Func<string, T> _parser;

        public FuncConverter(Func<string, T> parser)
        {
            _parser = parser;
        }

        protected override T Parse(string value)
        {
            return _parser.Invoke(value);
        }
    }

    public abstract class SimpleConverter<T> : JsonConverter<T>
    {
        protected abstract T Parse(string value);

        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return Parse(reader.GetString());
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }

        public override T ReadAsPropertyName(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return Read(ref reader, typeToConvert, options);
        }
        public override void WriteAsPropertyName(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            writer.WritePropertyName(value.ToString());
        }
    }
}
