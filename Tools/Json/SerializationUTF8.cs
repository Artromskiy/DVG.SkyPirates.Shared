using DVG.SkyPirates.Shared.Ids;
using System;
using System.Buffers;
using System.Globalization;
using System.Text;
using System.Text.Encodings.Web;
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
            _options.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
            _options.WriteIndented = true;
            _options.TypeInfoResolver = new DataContractResolver();

            var format = CultureInfo.InvariantCulture;
            _options.Converters.Add(new FixConverter());
            _options.Converters.Add(new FuncConverter<bool2>(value => bool2.Parse(value)));
            _options.Converters.Add(new FuncConverter<bool3>(value => bool3.Parse(value)));
            _options.Converters.Add(new FuncConverter<bool4>(value => bool4.Parse(value)));
            _options.Converters.Add(new FuncConverter<fix2>(value => fix2.Parse(value, format)));
            _options.Converters.Add(new FuncConverter<fix3>(value => fix3.Parse(value, format)));
            _options.Converters.Add(new FuncConverter<fix4>(value => fix4.Parse(value, format)));
            _options.Converters.Add(new FuncConverter<int2>(value => int2.Parse(value, format)));
            _options.Converters.Add(new FuncConverter<int3>(value => int3.Parse(value, format)));
            _options.Converters.Add(new FuncConverter<int4>(value => int4.Parse(value, format)));
            _options.Converters.Add(new FuncConverter<uint2>(value => uint2.Parse(value, format)));
            _options.Converters.Add(new FuncConverter<uint3>(value => uint3.Parse(value, format)));
            _options.Converters.Add(new FuncConverter<uint4>(value => uint4.Parse(value, format)));
            _options.Converters.Add(new FuncConverter<float2>(value => float2.Parse(value, format)));
            _options.Converters.Add(new FuncConverter<float3>(value => float3.Parse(value, format)));
            _options.Converters.Add(new FuncConverter<float4>(value => float4.Parse(value, format)));
            _options.Converters.Add(new FuncConverter<double2>(value => double2.Parse(value, format)));
            _options.Converters.Add(new FuncConverter<double3>(value => double3.Parse(value, format)));
            _options.Converters.Add(new FuncConverter<double4>(value => double4.Parse(value, format)));
            _options.Converters.Add(new FuncConverter<CheatingId>(value => new CheatingId(value)));
            _options.Converters.Add(new FuncConverter<GoodsId>(value => new GoodsId(value)));
            _options.Converters.Add(new FuncConverter<StateId>(value => new StateId(value)));
            _options.Converters.Add(new FuncConverter<TileId>(value => new TileId(value)));
            _options.Converters.Add(new FuncConverter<UnitId>(value => new UnitId(value)));
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
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            });
            JsonSerializer.Serialize(_writer, data, _options);
        }


        public static T Deserialize<T>(ReadOnlyMemory<byte> data)
        {
            var reader = new Utf8JsonReader(data.Span);
            return JsonSerializer.Deserialize<T>(ref reader, _options);
        }
    }
}