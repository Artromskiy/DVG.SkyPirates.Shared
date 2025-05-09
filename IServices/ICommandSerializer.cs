using System;

namespace DVG.SkyPirates.Shared.IServices
{
    public interface ICommandSerializer
    {
        ReadOnlySpan<byte> Serialize<T>(ref T data) where T : unmanaged;
        T Deserialize<T>(ReadOnlySpan<byte> data) where T : unmanaged;
    }
}
