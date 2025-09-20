using DVG.Core;
using System;
using System.Buffers;

namespace DVG.SkyPirates.Shared.IServices
{
    public interface ICommandSerializer
    {
        void Serialize<T>(IBufferWriter<byte> buffer, ref Command<T> data) where T : ICommandData;
        Command<T> Deserialize<T>(ReadOnlyMemory<byte> data) where T : ICommandData;
    }
}
