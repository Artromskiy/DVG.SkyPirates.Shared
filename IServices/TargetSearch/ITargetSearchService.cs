using Arch.Core;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.IServices.TargetSearch
{
    public interface ITargetSearchService: ITickableExecutor
    {
        Entity FindTarget(fix3 position, fix distance, int teamId);
        void FindTargets(fix3 position, fix distance, int teamId, List<Entity> targets);
    }
}
