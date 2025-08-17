using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.IServices.TargetSearch
{
    public interface ITargetSearchService: ITickableExecutor
    {
        ITarget? FindTarget(fix3 position, fix distance, int teamId);
        void FindTargets(fix3 position, fix distance, int teamId, List<ITarget> targets);
    }
}
