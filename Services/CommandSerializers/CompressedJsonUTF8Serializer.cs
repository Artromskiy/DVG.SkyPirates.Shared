using CommunityToolkit.HighPerformance;
using DVG.Commands;
using DVG.SkyPirates.Shared.Commands;
using DVG.SkyPirates.Shared.IServices;
using DVG.SkyPirates.Shared.Tools.Json;
using System;
using System.Buffers;
using System.IO.Compression;
using System.Text;

namespace DVG.SkyPirates.Shared.Services.CommandSerializers
{
    [Obsolete("Use SerializationUTF8")]
    public class CompressedJsonUTF8Serializer : ICommandSerializer
    {
        private readonly ArrayBufferWriter<byte> _buffer = new();

        public Command<T> Deserialize<T>(ReadOnlyMemory<byte> data)
        {
            _buffer.Clear();
            Decompress(data, _buffer);
            return SerializationUTF8.Deserialize<Command<T>>(_buffer.WrittenMemory);
            if (typeof(T) == typeof(LoadWorldCommand))
            {
                Console.WriteLine(Encoding.UTF8.GetString(_buffer.WrittenSpan));
            }
        }

        public void Serialize<T>(IBufferWriter<byte> buffer, ref Command<T> data)
        {
            _buffer.Clear();
            SerializationUTF8.Serialize(data, _buffer);
            //Console.WriteLine(Encoding.UTF8.GetString(_buffer.WrittenSpan));
            Compress(_buffer.WrittenMemory, buffer);
        }

        private void Compress(ReadOnlyMemory<byte> from, IBufferWriter<byte> to)
        {
            using var output = to.AsStream();
            using var input = from.AsStream();
            using DeflateStream dstream = new DeflateStream(output, CompressionLevel.Fastest);
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