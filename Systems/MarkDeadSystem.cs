using Arch.Core;
using DVG.Components;
using DVG.SkyPirates.Shared.Components.Runtime;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;
using System;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Systems
{
    [Obsolete]
    public sealed class MarkDeadSystem : IDeltaTickableExecutor
    {
        private readonly QueryDescription _desc = new QueryDescription().WithAll<Health>().
            NotDisposing().NotDisabled();

        private readonly List<Entity> _dead = new();

        private readonly World _world;
        public MarkDeadSystem(World world)
        {
            _world = world;
        }

        public void Tick(int tick, fix deltaTime)
        {
            _dead.Clear();
            var query = new SelectDeadQuery(_dead);
            _world.InlineEntityQuery<SelectDeadQuery, Health>(_desc, ref query);
            foreach (var item in _dead)
            {
                _world.Add<Disposing>(item);
            }
        }

        private readonly struct SelectDeadQuery : IForEachWithEntity<Health>
        {
            private readonly List<Entity> _dead;

            public SelectDeadQuery(List<Entity> deadUnits)
            {
                _dead = deadUnits;
            }

            public readonly void Update(Entity entity, ref Health health)
            {
                if (health <= fix.Zero)
                    _dead.Add(entity);
            }
        }
    }
}