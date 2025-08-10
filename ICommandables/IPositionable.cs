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
    public interface IPositionable : ICommandable<Position>
    {
        void SetPosition(real3 position);
        void ICommandable<Position>.Execute(Position cmd) => SetPosition(cmd.position);
    }
}
