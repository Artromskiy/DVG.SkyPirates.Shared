using DVG.Core;
using DVG.SkyPirates.Shared.IServices;
using System;
using System.Runtime.InteropServices;

namespace DVG.SkyPirates.Shared.Services
{
    public class CommandSerializer : ICommandSerializer
    {
        public ReadOnlySpan<byte> Serialize<T>(ref Command<T> data)
            where T : unmanaged, ICommandData
        {
            var span = MemoryMarshal.CreateReadOnlySpan(ref data, 1);
            ReadOnlySpan<byte> spanBytes = MemoryMarshal.AsBytes(span);
            return spanBytes;
        }

        public Command<T> Deserialize<T>(ReadOnlySpan<byte> data)
            where T : unmanaged, ICommandData
        {
            return MemoryMarshal.Read<Command<T>>(data);
        }
    }
}
