#region Reals
using real = System.Single;
using real2 = DVG.float2;
using real3 = DVG.float3;
using real4 = DVG.float4;
#endregion

using DVG.Core;
using DVG.SkyPirates.Shared.IViewModels;

namespace DVG.SkyPirates.Shared.IViews
{
    public interface IUnitView: IView<IUnitVM>
    {
        real3 Position { get; }
        real Rotation { get; }
        real PreAttack { get; }
        real PostAttack { get; }
    }
}
