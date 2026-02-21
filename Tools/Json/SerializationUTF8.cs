using DVG.SkyPirates.Shared.Components.Config;
using DVG.SkyPirates.Shared.Components.Runtime;
using DVG.SkyPirates.Shared.Ids;
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
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                TypeInfoResolver = new DataContractResolver(),
            };
            var format = CultureInfo.InvariantCulture;

            Options.Converters.Add(new FixConverter());

            Options.Converters.Add(new StringFuncConverter<bool2>(value => bool2.Parse(value)));
            Options.Converters.Add(new StringFuncConverter<bool3>(value => bool3.Parse(value)));
            Options.Converters.Add(new StringFuncConverter<bool4>(value => bool4.Parse(value)));
            Options.Converters.Add(new StringFuncConverter<fix2>(value => fix2.Parse(value, format)));
            Options.Converters.Add(new StringFuncConverter<fix3>(value => fix3.Parse(value, format)));
            Options.Converters.Add(new StringFuncConverter<fix4>(value => fix4.Parse(value, format)));
            Options.Converters.Add(new StringFuncConverter<int2>(value => int2.Parse(value, format)));
            Options.Converters.Add(new StringFuncConverter<int3>(value => int3.Parse(value, format)));
            Options.Converters.Add(new StringFuncConverter<int4>(value => int4.Parse(value, format)));
            Options.Converters.Add(new StringFuncConverter<uint2>(value => uint2.Parse(value, format)));
            Options.Converters.Add(new StringFuncConverter<uint3>(value => uint3.Parse(value, format)));
            Options.Converters.Add(new StringFuncConverter<uint4>(value => uint4.Parse(value, format)));
            Options.Converters.Add(new StringFuncConverter<float2>(value => float2.Parse(value, format)));
            Options.Converters.Add(new StringFuncConverter<float3>(value => float3.Parse(value, format)));
            Options.Converters.Add(new StringFuncConverter<float4>(value => float4.Parse(value, format)));
            Options.Converters.Add(new StringFuncConverter<double2>(value => double2.Parse(value, format)));
            Options.Converters.Add(new StringFuncConverter<double3>(value => double3.Parse(value, format)));
            Options.Converters.Add(new StringFuncConverter<double4>(value => double4.Parse(value, format)));

            // NewType implicit casting used
            Options.Converters.Add(new FixFuncConverter<Health>(value => value, value => value));
            Options.Converters.Add(new FixFuncConverter<Radius>(value => value, value => value));
            Options.Converters.Add(new FixFuncConverter<Damage>(value => value, value => value));
            Options.Converters.Add(new FixFuncConverter<MaxSpeed>(value => value, value => value));
            Options.Converters.Add(new FixFuncConverter<MaxHealth>(value => value, value => value));
            Options.Converters.Add(new FixFuncConverter<Separation>(value => value, value => value));
            Options.Converters.Add(new FixFuncConverter<ImpactDistance>(value => value, value => value));
            Options.Converters.Add(new FixFuncConverter<GoodsCollectorRadius>(value => value, value => value));
            Options.Converters.Add(new FixFuncConverter<TargetSearchDistance>(value => value, value => value));

            Options.Converters.Add(new IdConverter<CheatingId>());
            Options.Converters.Add(new IdConverter<GoodsId>());
            Options.Converters.Add(new IdConverter<StateId>());
            Options.Converters.Add(new IdConverter<TileId>());
            Options.Converters.Add(new IdConverter<UnitId>());
            Options.Converters.Add(new IdConverter<CactusId>());
            Options.Converters.Add(new IdConverter<TreeId>());
            Options.Converters.Add(new IdConverter<RockId>());
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