using DVG.NewType;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DVG.SkyPirates.Shared.Tools.Json
{
    public class NewTypeConverterFactory : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert) =>
            typeof(INewType).IsAssignableFrom(typeToConvert);

        public override JsonConverter CreateConverter(Type type, JsonSerializerOptions options)
        {
            var valueType = type.GetProperty("Value").PropertyType;
            var converterType = typeof(NewTypeConverter<,>).MakeGenericType(type, valueType);
            return (JsonConverter)Activator.CreateInstance(converterType);
        }
    }

    public sealed class NewTypeConverter<T, V> : JsonConverter<T>
        where T : INewType<V>
    {
        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = JsonSerializer.Deserialize<V>(ref reader, options);
            var newType = Activator.CreateInstance<T>();
            newType.Value = value!;
            return newType;
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value.Value, options);
            writer.WriteStringValue(value.ToString());
        }

        public override T ReadAsPropertyName(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return Read(ref reader, typeToConvert, options);
        }

        public override void WriteAsPropertyName(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            writer.WritePropertyName(JsonSerializer.Serialize(value, options));
        }
    }
}
