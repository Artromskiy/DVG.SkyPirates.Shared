using Arch.Core;
using DVG.SkyPirates.Shared.Components;
using DVG.SkyPirates.Shared.Components.Data;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.IServices
{
    public interface ITargetSearchSystem : ITickableExecutor
    {
        Entity FindTarget(ref TargetSearchData targetSearchData, ref Team team);
        void FindTargets(ref TargetSearchData targetSearchData, ref Team team, List<(Entity, Position)> targets);
    }
}