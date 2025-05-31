using DVG.Core;
using System;

namespace DVG.SkyPirates.Shared.IServices
{
    public interface ICommandSerializer
    {
        ReadOnlySpan<byte> Serialize<T>(ref Command<T> data) where T : unmanaged, ICommandData;
        Command<T> Deserialize<T>(ReadOnlySpan<byte> data) where T : unmanaged, ICommandData;
    }
}
