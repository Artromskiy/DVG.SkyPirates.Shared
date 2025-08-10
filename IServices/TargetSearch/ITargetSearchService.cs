#region Reals
using real = System.Single;
using real2 = DVG.float2;
using real3 = DVG.float3;
using real4 = DVG.float4;
#endregion

namespace DVG.SkyPirates.Shared.IServices.TargetSearch
{
    public interface ITargetSearchService
    {
        ITarget FindTarget(real3 position, real distance);
        ITarget[] FindTargets(real3 position, real distance);
        void UpdateTargetsSearch();
    }
}
