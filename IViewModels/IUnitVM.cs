#region Reals
using real = System.Single;
using real2 = DVG.float2;
using real3 = DVG.float3;
using real4 = DVG.float4;
#endregion

using DVG.Core;

namespace DVG.SkyPirates.Shared.IViewModels
{
    public interface IUnitVM : IViewModel
    {
        public real3 Position { get; }
        public real Rotation { get; }
        public real PreAttack { get; }
        public real PostAttack { get; }
    }
}
