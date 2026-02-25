using System;
using System.Buffers;
using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DVG.SkyPirates.Shared.Tools.Json
{
    public static class SerializationUTF8
    {
        private static Utf8JsonWriter? _writer;
        private static readonly ArrayBufferWriter<byte> _buffer;
        public static readonly JsonSerializerOptions Options;
        static SerializationUTF8()
        {
            _buffer = new ArrayBufferWriter<byte>();
            Options = new(JsonSerializerOptions.Default)
            {
                IncludeFields = true,
                IgnoreReadOnlyFields = false,
                IgnoreReadOnlyProperties = false,
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
                TypeInfoResolver = new DataContractResolver(),
            };
            var format = CultureInfo.InvariantCulture;

            Options.Converters.Add(new FixConverter());
            Options.Converters.Add(new IdConverterFactory());
            Options.Converters.Add(new NewTypeConverterFactory());
            Options.Converters.Add(new FrozenDictionaryConverterFactory());

            Options.Converters.Add(new VectorConverter<fix, fix2>());
            Options.Converters.Add(new VectorConverter<fix, fix3>());
            Options.Converters.Add(new VectorConverter<fix, fix4>());
            Options.Converters.Add(new VectorConverter<bool, bool2>());
            Options.Converters.Add(new VectorConverter<bool, bool3>());
            Options.Converters.Add(new VectorConverter<bool, bool4>());
            Options.Converters.Add(new VectorConverter<int, int2>());
            Options.Converters.Add(new VectorConverter<int, int3>());
            Options.Converters.Add(new VectorConverter<int, int4>());
            Options.Converters.Add(new VectorConverter<uint, uint2>());
            Options.Converters.Add(new VectorConverter<uint, uint3>());
            Options.Converters.Add(new VectorConverter<uint, uint4>());
            Options.Converters.Add(new VectorConverter<float, float2>());
            Options.Converters.Add(new VectorConverter<float, float3>());
            Options.Converters.Add(new VectorConverter<float, float4>());
            Options.Converters.Add(new VectorConverter<double, double2>());
            Options.Converters.Add(new VectorConverter<double, double3>());
            Options.Converters.Add(new VectorConverter<double, double4>());
        }

        public static string Serialize<T>(T data)
        {
            _buffer.Clear();
            Serialize(data, _buffer);

            return Encoding.UTF8.GetString(_buffer.WrittenSpan);
        }

        public static T Deserialize<T>(string json)
        {
            _buffer.Clear();
            int count = Encoding.UTF8.GetByteCount(json);
            count = Encoding.UTF8.GetBytes(json, _buffer.GetSpan(count));
            _buffer.Advance(count);

            return Deserialize<T>(_buffer.WrittenMemory);
        }

        public static void Serialize<T>(T data, IBufferWriter<byte> buffer)
        {
            _writer?.Reset(buffer);
            _writer ??= new Utf8JsonWriter(buffer, new JsonWriterOptions()
            {
                Indented = true,
                SkipValidation = true,
            });
            JsonSerializer.Serialize(_writer, data, Options);
        }


        public static T Deserialize<T>(ReadOnlyMemory<byte> data)
        {
            var reader = new Utf8JsonReader(data.Span);
            return JsonSerializer.Deserialize<T>(ref reader, Options);
        }
    }
}