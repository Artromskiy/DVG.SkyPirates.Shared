using Arch.Core;
using DVG.SkyPirates.Shared.Components;
using DVG.SkyPirates.Shared.Components.Data;
using DVG.SkyPirates.Shared.Components.Special;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Systems
{
    public sealed class MarkDeadSystem : ITickableExecutor
    {
        private readonly QueryDescription _desc = new QueryDescription().WithAll<Health, Alive>();

        private readonly List<Entity> _dead = new List<Entity>();

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
                _world.Remove<Alive>(item);
                _world.Add<Destruct>(item);
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
                if (health.Value <= 0)
                    _dead.Add(entity);
            }
        }
    }
}