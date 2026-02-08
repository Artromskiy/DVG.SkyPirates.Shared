using Arch.Core;
using DVG.SkyPirates.Shared.Components.Special;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Systems.Special
{
    internal class DisposeSystem : ITickableExecutor
    {
        private readonly QueryDescription _desc = new QueryDescription().WithAll<Dispose>().NotDisposing();
        private readonly List<Entity> _entitiesCache = new();
        private readonly World _world;

        public DisposeSystem(World world)
        {
            _world = world;
        }

        public void Tick(int tick, fix deltaTime)
        {
            _entitiesCache.Clear();
            var query = new SelectToDispose(_entitiesCache);
            _world.InlineEntityQuery<SelectToDispose, Dispose>(_desc, ref query);
            foreach (var entity in _entitiesCache)
            {
                _world.Destroy(entity);
            }
        }

        private readonly struct SelectToDispose : IForEachWithEntity<Dispose>
        {
            private readonly List<Entity> _entities;

            public SelectToDispose(List<Entity> entities)
            {
                _entities = entities;
            }

            public void Update(Entity entity, ref Dispose destruct)
            {
                if (++destruct.TicksPassed > Constants.HistoryTicks)
                    _entities.Add(entity);
            }
        }
    }
}
