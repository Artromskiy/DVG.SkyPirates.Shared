using Arch.Core;
using DVG.SkyPirates.Shared.Components;
using DVG.SkyPirates.Shared.Components.Data;
using DVG.SkyPirates.Shared.IServices;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;

namespace DVG.SkyPirates.Shared.Systems
{
    /// <summary>
    /// Sets value to <see href="Target"/> if it matches <see href="TargetSearchData"/> and <see href="TeamId"/> conditions
    /// </summary>
    public sealed class SetTargetSystem : ITickableExecutor
    {
        private readonly QueryDescription _desc = new QueryDescription().
            WithAll<Position, Target, Team>().
            WithNone<Dead>();

        private readonly World _world;
        private readonly ITargetSearchSystem _targetSearch;
        public SetTargetSystem(World world, ITargetSearchSystem targetSearch)
        {
            _world = world;
            _targetSearch = targetSearch;
        }

        public void Tick(int tick, fix deltaTime)
        {
            var query = new SetTargetQuery(_targetSearch);
            _world.InlineQuery<SetTargetQuery, TargetSearchData, Target, Team>(_desc, ref query);
        }

        private readonly struct SetTargetQuery :
            IForEach<TargetSearchData, Target, Team>
        {
            private readonly ITargetSearchSystem _targetSearch;

            public SetTargetQuery(ITargetSearchSystem targetSearch)
            {
                _targetSearch = targetSearch;
            }

            public void Update(ref TargetSearchData targetSearchData, ref Target target, ref Team team)
            {
                target.Entity = _targetSearch.FindTarget(ref targetSearchData, ref team);
            }
        }
    }
}
