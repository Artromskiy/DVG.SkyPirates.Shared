using Arch.Core;
using DVG.SkyPirates.Shared.Components.Config;
using DVG.SkyPirates.Shared.Components.Framed;
using DVG.SkyPirates.Shared.Components.Runtime;
using DVG.SkyPirates.Shared.IServices;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Systems
{
    public class SetMultiTargetSystem : ITickableExecutor
    {
        private readonly QueryDescription _desc = new QueryDescription().
            WithAll<Position, Targets, TargetSearchDistance, TargetSearchPosition, TeamId>().NotDisposing();

        private readonly World _world;
        private readonly ITargetSearchSystem _targetSearch;

        private readonly List<Entity> _targetsCache = new();

        public SetMultiTargetSystem(World world, ITargetSearchSystem targetSearch)
        {
            _world = world;
            _targetSearch = targetSearch;
        }

        public void Tick(int tick, fix deltaTime)
        {
            var query = new SetTargetQuery(_targetSearch, _targetsCache);
            _world.InlineQuery<SetTargetQuery, TargetSearchDistance, TargetSearchPosition, Targets, TeamId>(_desc, ref query);
        }

        private readonly struct SetTargetQuery :
            IForEach<TargetSearchDistance, TargetSearchPosition, Targets, TeamId>
        {
            private readonly ITargetSearchSystem _targetSearch;
            private readonly List<Entity> _targetsCache;

            public SetTargetQuery(ITargetSearchSystem targetSearch, List<Entity> targetsCache)
            {
                _targetSearch = targetSearch;
                _targetsCache = targetsCache;
            }

            public void Update(ref TargetSearchDistance searchDistance, ref TargetSearchPosition searchPosition, ref Targets target, ref TeamId team)
            {
                _targetsCache.Clear();
                _targetSearch.FindTargets(ref searchDistance, ref searchPosition, ref team, _targetsCache);
                if (_targetsCache.Count > 0)
                {
                    target.Entities = new(_targetsCache);
                }
            }
        }
    }
}
