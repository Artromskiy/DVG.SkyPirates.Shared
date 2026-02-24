using DVG.Ids;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DVG.SkyPirates.Shared.Tools.Json
{
    public class IdConverterFactory : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert) =>
            typeToConvert.IsValueType &&
            typeof(IId).IsAssignableFrom(typeToConvert);

        public override JsonConverter CreateConverter(Type type, JsonSerializerOptions options)
        {
            var converterType = typeof(IdConverter<>).MakeGenericType(type);
            return (JsonConverter)Activator.CreateInstance(converterType);
        }
    }

    public sealed class IdConverter<T> : JsonConverter<T>
        where T : struct, IId
    {
        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return (T)Activator.CreateInstance(typeof(T), reader.GetString());
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
