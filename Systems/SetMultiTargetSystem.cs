using Arch.Core;
using DVG.SkyPirates.Shared.Components;
using DVG.SkyPirates.Shared.Components.Data;
using DVG.SkyPirates.Shared.IServices;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Systems
{
    public class SetMultiTargetSystem : ITickableExecutor
    {
        private readonly QueryDescription _desc = new QueryDescription().
            WithAll<Position, Targets, Team, Alive>();

        private readonly World _world;
        private readonly ITargetSearchSystem _targetSearch;

        private readonly List<(Entity, Position)> _targetsCache = new();

        public SetMultiTargetSystem(World world, ITargetSearchSystem targetSearch)
        {
            _world = world;
            _targetSearch = targetSearch;
        }

        public void Tick(int tick, fix deltaTime)
        {
            var query = new SetTargetQuery(_targetSearch, _targetsCache);
            _world.InlineQuery<SetTargetQuery, TargetSearchData, Targets, Team>(_desc, ref query);
        }

        private readonly struct SetTargetQuery :
            IForEach<TargetSearchData, Targets, Team>
        {
            private readonly ITargetSearchSystem _targetSearch;
            private readonly List<(Entity target, Position position)> _targetsCache;

            public SetTargetQuery(ITargetSearchSystem targetSearch, List<(Entity, Position)> targetsCache)
            {
                _targetSearch = targetSearch;
                _targetsCache = targetsCache;
            }

            public void Update(ref TargetSearchData targetSearchData, ref Targets target, ref Team team)
            {
                _targetsCache.Clear();
                _targetSearch.FindTargets(ref targetSearchData, ref team, _targetsCache);
                target.Entities = new();
                foreach (var item in _targetsCache)
                    target.Entities.Add(item.target);
            }
        }
    }
}
