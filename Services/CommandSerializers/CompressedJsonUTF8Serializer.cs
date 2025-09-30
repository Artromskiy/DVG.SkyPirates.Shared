using CommunityToolkit.HighPerformance;
using DVG.Core;
using DVG.SkyPirates.Shared.IServices;
using DVG.SkyPirates.Shared.Tools.Json;
using System;
using System.Buffers;
using System.IO.Compression;

namespace DVG.SkyPirates.Shared.Services.CommandSerializers
{
    public class CompressedJsonUTF8Serializer : ICommandSerializer
    {
        private readonly ArrayBufferWriter<byte> _buffer = new ArrayBufferWriter<byte>();

        public Command<T> Deserialize<T>(ReadOnlyMemory<byte> data) where T : ICommandData
        {
            _buffer.Clear();
            Decompress(data, _buffer);
            return SerializationUTF8.Deserialize<Command<T>>(_buffer.WrittenMemory);
        }

        public void Serialize<T>(IBufferWriter<byte> buffer, ref Command<T> data) where T : ICommandData
        {
            _buffer.Clear();
            SerializationUTF8.Serialize(data, _buffer);
            Compress(_buffer, buffer);
        }

        public void Compress(IBufferWriter<byte> from, IBufferWriter<byte> to)
        {
            _buffer.Clear();
            using var output = to.AsStream();
            using var input = from.AsStream();
            using DeflateStream dstream = new DeflateStream(input, CompressionLevel.Optimal);
            dstream.CopyTo(output);
        }

        public static void Decompress(ReadOnlyMemory<byte> from, IBufferWriter<byte> to)
        {
            using var input = from.AsStream();
            using var output = to.AsStream();
            using DeflateStream dstream = new(input, CompressionMode.Decompress);
            dstream.CopyTo(output);
        }
    }
}
