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
    public interface IRotationable : ICommandable<Rotation>
    {
        void SetRotation(real rotation);
        void ICommandable<Rotation>.Execute(Rotation cmd) => SetRotation(cmd.rotation);
    }
}
