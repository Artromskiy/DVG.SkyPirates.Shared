using DVG.Commands;
using System;
using System.Buffers;

namespace DVG.SkyPirates.Shared.IServices
{
    public interface ICommandSerializer
    {
        void Serialize<T>(IBufferWriter<byte> buffer, ref Command<T> data);
        Command<T> Deserialize<T>(ReadOnlyMemory<byte> data);
    }
}
