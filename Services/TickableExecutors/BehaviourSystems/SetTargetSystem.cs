using Arch.Core;
using DVG.SkyPirates.Shared.Components;
using DVG.SkyPirates.Shared.Components.Data;
using DVG.SkyPirates.Shared.IServices.TargetSearch;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;

namespace DVG.SkyPirates.Shared.Services.TickableExecutors.BehaviourSystems
{
    /// <summary>
    /// Sets value to <see href="Target"/> if it matches <see href="TargetSearchData"/> and <see href="TeamId"/> conditions
    /// </summary>
    public class SetTargetSystem : ITickableExecutor
    {
        private readonly World _world;
        private readonly ITargetSearchService _targetSearch;
        public SetTargetSystem(World world, ITargetSearchService targetSearch)
        {
            _world = world;
            _targetSearch = targetSearch;
        }

        public void Tick(int tick, fix deltaTime)
        {
            var desc = new QueryDescription().WithAll<
                Position,
                Target,
                Team>();

            var query = new SetTargetQuery(_targetSearch);
            _world.InlineQuery<SetTargetQuery, TargetSearchData, Target, Team>(desc, ref query);
        }

        private readonly struct SetTargetQuery :
            IForEach<TargetSearchData, Target, Team>
        {
            private readonly ITargetSearchService _targetSearch;

            public SetTargetQuery(ITargetSearchService targetSearch)
            {
                _targetSearch = targetSearch;
            }

            public void Update(ref TargetSearchData targetSearchData, ref Target target, ref Team team)
            {
                target.Entity = _targetSearch.FindTarget(targetSearchData.Position, targetSearchData.Distance, team.Id);
            }
        }
    }
}
