using Arch.Core;
using DVG.SkyPirates.Shared.Components.Special;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Systems.Special
{
    internal class DestructSystem : ITickableExecutor
    {
        private readonly QueryDescription _desc = new QueryDescription().WithAll<Destruct, History<Destruct>>();
        private readonly List<Entity> _entitiesCache = new();
        private readonly World _world;

        public DestructSystem(World world)
        {
            _world = world;
        }

        public void Tick(int tick, fix deltaTime)
        {
            _entitiesCache.Clear();
            var query = new SelectToDestruct(_entitiesCache);
            _world.InlineEntityQuery<SelectToDestruct, History<Destruct>>(_desc, ref query);
            foreach (var entity in _entitiesCache)
            {
                _world.RemoveRange(entity, _world.GetSignature(entity).Components);
            }
        }

        private readonly struct SelectToDestruct : IForEachWithEntity<History<Destruct>>
        {
            private readonly List<Entity> _entities;

            public SelectToDestruct(List<Entity> entities)
            {
                _entities = entities;
            }

            public void Update(Entity entity, ref History<Destruct> history)
            {
                if (history.AllHasValues())
                    _entities.Add(entity);
            }
        }
    }
}
