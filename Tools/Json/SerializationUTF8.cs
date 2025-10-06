using DVG.SkyPirates.Shared.Ids;
using System;
using System.Buffers;
using System.Globalization;
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
            var format = CultureInfo.InvariantCulture;
            _options.TypeInfoResolver = new DataContractResolver();
            _options.Converters.Add(new Converter<bool2>(value => bool2.Parse(value)));
            _options.Converters.Add(new Converter<bool3>(value => bool3.Parse(value)));
            _options.Converters.Add(new Converter<bool4>(value => bool4.Parse(value)));
            _options.Converters.Add(new Converter<fix2>(value => fix2.Parse(value, format)));
            _options.Converters.Add(new Converter<fix3>(value => fix3.Parse(value, format)));
            _options.Converters.Add(new Converter<fix4>(value => fix4.Parse(value, format)));
            _options.Converters.Add(new Converter<int2>(value => int2.Parse(value, format)));
            _options.Converters.Add(new Converter<int3>(value => int3.Parse(value, format)));
            _options.Converters.Add(new Converter<int4>(value => int4.Parse(value, format)));
            _options.Converters.Add(new Converter<uint2>(value => uint2.Parse(value, format)));
            _options.Converters.Add(new Converter<uint3>(value => uint3.Parse(value, format)));
            _options.Converters.Add(new Converter<uint4>(value => uint4.Parse(value, format)));
            _options.Converters.Add(new Converter<float2>(value => float2.Parse(value, format)));
            _options.Converters.Add(new Converter<float3>(value => float3.Parse(value, format)));
            _options.Converters.Add(new Converter<float4>(value => float4.Parse(value, format)));
            _options.Converters.Add(new Converter<double2>(value => double2.Parse(value, format)));
            _options.Converters.Add(new Converter<double3>(value => double3.Parse(value, format)));
            _options.Converters.Add(new Converter<double4>(value => double4.Parse(value, format)));
            _options.Converters.Add(new Converter<CheatingId>(value => new CheatingId(value)));
            _options.Converters.Add(new Converter<GoodsId>(value => new GoodsId(value)));
            _options.Converters.Add(new Converter<StateId>(value => new StateId(value)));
            _options.Converters.Add(new Converter<TileId>(value => new TileId(value)));
            _options.Converters.Add(new Converter<UnitId>(value => new UnitId(value)));
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
            _writer ??= new Utf8JsonWriter(buffer);
            JsonSerializer.Serialize(_writer, data, _options);
        }


        public static T Deserialize<T>(ReadOnlyMemory<byte> data)
        {
            var reader = new Utf8JsonReader(data.Span);
            return JsonSerializer.Deserialize<T>(ref reader, _options);
        }


        private class Converter<T> : SimpleConverter<T>
        {
            private readonly Func<string, T> _parser;

            public Converter(Func<string, T> parser)
            {
                _parser = parser;
            }

            protected override T Parse(string value)
            {
                return _parser.Invoke(value);
            }
        }
    }
}
