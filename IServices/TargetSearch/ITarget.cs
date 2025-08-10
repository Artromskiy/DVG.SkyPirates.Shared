#region Reals
using real = System.Single;
using real2 = DVG.float2;
using real3 = DVG.float3;
using real4 = DVG.float4;
#endregion

namespace DVG.SkyPirates.Shared.IServices.TargetSearch
{
    public interface ITarget
    {
        public real3 Position { get; }
        public int Health { get; }
    }
}
