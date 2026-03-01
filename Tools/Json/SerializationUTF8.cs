using CommunityToolkit.HighPerformance;
using System;
using System.Buffers;
using System.IO.Compression;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

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
            Options = new(JsonSerializerDefaults.Strict)
            {
                IncludeFields = true,
                IgnoreReadOnlyFields = false,
                IgnoreReadOnlyProperties = false,
                WriteIndented = true,
                AllowDuplicateProperties = false,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
                TypeInfoResolver = new DefaultJsonTypeInfoResolver().
                WithAddedModifier(IgnoreMod.Modify).
                WithAddedModifier(OrderMod.Modify),
            };

            Options.Converters.Add(new FixConverter());
            Options.Converters.Add(new IdConverterFactory());
            Options.Converters.Add(new NewTypeConverterFactory());
            Options.Converters.Add(new FrozenDictionaryConverterFactory());
            Options.Converters.Add(new ImmutableSortedDictionaryConverterFactory());

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
            _writer ??= new Utf8JsonWriter(buffer, new JsonWriterOptions()
            {
                Indented = true,
                SkipValidation = true,
            });
            _writer?.Reset(buffer);
            JsonSerializer.Serialize(_writer, data, Options);
        }

        public static T Deserialize<T>(ReadOnlyMemory<byte> data)
        {
            var reader = new Utf8JsonReader(data.Span);
            return JsonSerializer.Deserialize<T>(ref reader, Options);
        }

        public static T DeserializeCompressed<T>(ReadOnlyMemory<byte> data)
        {
            _buffer.Clear();
            Decompress(data, _buffer);
            return Deserialize<T>(_buffer.WrittenMemory);
        }

        public static void SerializeCompressed<T>(IBufferWriter<byte> buffer, T data)
        {
            _buffer.Clear();
            Serialize(data, _buffer);
            Compress(_buffer.WrittenMemory, buffer);
        }

        public static string SerializeOrdered<T>(T data)
        {
            var document = JsonSerializer.SerializeToDocument(data, Options);
            _buffer.Clear();
            _writer.Reset(_buffer);
            _writer.WriteOrdered(document.RootElement);
            _writer.Flush();
            return Encoding.UTF8.GetString(_buffer.WrittenSpan);
        }

        private static void Compress(ReadOnlyMemory<byte> from, IBufferWriter<byte> to)
        {
            using var output = to.AsStream();
            using var input = from.AsStream();
            using DeflateStream dstream = new(output, CompressionLevel.Fastest);
            dstream.Write(from.Span);
        }

        private static void Decompress(ReadOnlyMemory<byte> from, IBufferWriter<byte> to)
        {
            using var input = from.AsStream();
            using var output = to.AsStream();
            using DeflateStream dstream = new(input, CompressionMode.Decompress);
            dstream.CopyTo(output);
        }
    }
}