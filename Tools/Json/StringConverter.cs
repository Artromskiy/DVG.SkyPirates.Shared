using DVG.Core;
using DVG.SkyPirates.Shared.Ids;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DVG.SkyPirates.Shared.Tools.Json
{
    public class IdConverter<T> : StringConverter<T> where T : struct, IId
    {
        protected override T Parse(string value) => IdFactory.Create<T>(value);
    }

    public class StringFuncConverter<T> : StringConverter<T>
    {
        private readonly Func<string, T> _parser;

        public StringFuncConverter(Func<string, T> parser)
        {
            _parser = parser;
        }
        protected override T Parse(string value) => _parser.Invoke(value);
    }

    public class FixFuncConverter<T> : NumberConverter<T>
    {
        private readonly Func<fix, T> _reader;
        private readonly Func<T, fix> _writer;

        public FixFuncConverter(Func<fix, T> reader, Func<T, fix> writer)
        {
            _reader = reader;
            _writer = writer;
        }

        protected override T Read(decimal value) => _reader.Invoke((fix)value);
        protected override decimal Write(T value) => (decimal)_writer.Invoke(value);
    }

    public abstract class StringConverter<T> : JsonConverter<T>
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

    public abstract class NumberConverter<T> : JsonConverter<T>
    {
        protected abstract T Read(decimal value);
        protected abstract decimal Write(T value);

        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return Read(reader.GetDecimal());
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(Write(value));
        }
    }
}
