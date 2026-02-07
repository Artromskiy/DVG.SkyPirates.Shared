using DVG.SkyPirates.Shared.Components.Config;
using DVG.SkyPirates.Shared.Components.Runtime;
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
            //_options.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
            _options.WriteIndented = true;
            _options.TypeInfoResolver = new DataContractResolver();

            var format = CultureInfo.InvariantCulture;

            _options.Converters.Add(new FixConverter());

            _options.Converters.Add(new StringFuncConverter<bool2>(value => bool2.Parse(value)));
            _options.Converters.Add(new StringFuncConverter<bool3>(value => bool3.Parse(value)));
            _options.Converters.Add(new StringFuncConverter<bool4>(value => bool4.Parse(value)));
            _options.Converters.Add(new StringFuncConverter<fix2>(value => fix2.Parse(value, format)));
            _options.Converters.Add(new StringFuncConverter<fix3>(value => fix3.Parse(value, format)));
            _options.Converters.Add(new StringFuncConverter<fix4>(value => fix4.Parse(value, format)));
            _options.Converters.Add(new StringFuncConverter<int2>(value => int2.Parse(value, format)));
            _options.Converters.Add(new StringFuncConverter<int3>(value => int3.Parse(value, format)));
            _options.Converters.Add(new StringFuncConverter<int4>(value => int4.Parse(value, format)));
            _options.Converters.Add(new StringFuncConverter<uint2>(value => uint2.Parse(value, format)));
            _options.Converters.Add(new StringFuncConverter<uint3>(value => uint3.Parse(value, format)));
            _options.Converters.Add(new StringFuncConverter<uint4>(value => uint4.Parse(value, format)));
            _options.Converters.Add(new StringFuncConverter<float2>(value => float2.Parse(value, format)));
            _options.Converters.Add(new StringFuncConverter<float3>(value => float3.Parse(value, format)));
            _options.Converters.Add(new StringFuncConverter<float4>(value => float4.Parse(value, format)));
            _options.Converters.Add(new StringFuncConverter<double2>(value => double2.Parse(value, format)));
            _options.Converters.Add(new StringFuncConverter<double3>(value => double3.Parse(value, format)));
            _options.Converters.Add(new StringFuncConverter<double4>(value => double4.Parse(value, format)));

            _options.Converters.Add(new FixFuncConverter<Health>(value => new() { Value = value }, value => value.Value));
            _options.Converters.Add(new FixFuncConverter<Radius>(value => new() { Value = value }, value => value.Value));
            _options.Converters.Add(new FixFuncConverter<Damage>(value => new() { Value = value }, value => value.Value));
            _options.Converters.Add(new FixFuncConverter<MaxSpeed>(value => new() { Value = value }, value => value.Value));
            _options.Converters.Add(new FixFuncConverter<MaxHealth>(value => new() { Value = value }, value => value.Value));
            _options.Converters.Add(new FixFuncConverter<ImpactDistance>(value => new() { Value = value }, value => value.Value));
            _options.Converters.Add(new FixFuncConverter<TargetSearchDistance>(value => new() { Value = value }, value => value.Value));

            _options.Converters.Add(new IdConverter<CheatingId>());
            _options.Converters.Add(new IdConverter<GoodsId>());
            _options.Converters.Add(new IdConverter<StateId>());
            _options.Converters.Add(new IdConverter<TileId>());
            _options.Converters.Add(new IdConverter<UnitId>());
            _options.Converters.Add(new IdConverter<CactusId>());
            _options.Converters.Add(new IdConverter<TreeId>());
            _options.Converters.Add(new IdConverter<RockId>());
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