#region Reals
using real = System.Single;
using real2 = DVG.float2;
using real3 = DVG.float3;
using real4 = DVG.float4;
#endregion

using DVG.Core;

namespace DVG.SkyPirates.Shared.IViews
{
    public interface IUnitView : IView
    {
        public real3 Position { get; set; }
        public real Rotation { get; set; }
        public real PreAttack { get; set; }
        public real PostAttack { get; set; }
    }
}
