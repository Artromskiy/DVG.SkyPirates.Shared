#region Reals
using real = System.Single;
using real2 = DVG.float2;
using real3 = DVG.float3;
using real4 = DVG.float4;
#endregion

using DVG.Core;
using DVG.SkyPirates.Shared.Commands;

namespace DVG.SkyPirates.Shared.ICommandables
{
    public interface IDirectionable : ICommandable<Direction>
    {
        void SetDirection(real2 direction);
        void ICommandable<Direction>.Execute(Direction cmd) => SetDirection(cmd.direction);
    }
}
