using DVG.SkyPirates.Shared.IServices;
using System;
using System.Runtime.InteropServices;

namespace DVG.SkyPirates.Shared.Services
{
    public class CommandSerializer : ICommandSerializer
    {
        public ReadOnlySpan<byte> Serialize<T>(ref T data) where T : unmanaged
        {
            var span = MemoryMarshal.CreateReadOnlySpan(ref data, 1);
            ReadOnlySpan<byte> spanBytes = MemoryMarshal.AsBytes(span);
            return spanBytes;
        }

        public T Deserialize<T>(ReadOnlySpan<byte> data) where T : unmanaged
        {
            return MemoryMarshal.Read<T>(data);
        }
    }
}
