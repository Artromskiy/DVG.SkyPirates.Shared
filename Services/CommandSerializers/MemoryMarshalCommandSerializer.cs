using DVG.Commands;
using DVG.SkyPirates.Shared.IServices;
using System;
using System.Buffers;
using System.Runtime.InteropServices;

namespace DVG.SkyPirates.Shared.Services.CommandSerializers
{
    public class MemoryMarshalCommandSerializer : ICommandSerializer
    {
        public void Serialize<T>(IBufferWriter<byte> buffer, ref Command<T> data)
        {
            var span = MemoryMarshal.CreateReadOnlySpan(ref data, 1);
            ReadOnlySpan<byte> spanBytes = MemoryMarshal.AsBytes(span);
            buffer.Write(spanBytes);
        }

        public Command<T> Deserialize<T>(ReadOnlyMemory<byte> data)
        {
            Command<T> result;
            try
            {
                result = MemoryMarshal.Read<Command<T>>(data.Span);
            }
            catch (Exception e)
            {
                Console.WriteLine(typeof(T));
                throw e;
            }
            return result;
        }
    }
}