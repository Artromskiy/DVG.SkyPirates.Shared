using Arch.Core;
using DVG.Components;
using DVG.Core.Collections;
using DVG.SkyPirates.Shared.IFactories;

namespace DVG.SkyPirates.Shared.Factories
{
    public class EntityFactory : IEntityFactory
    {
        private readonly World _world;

        private readonly Lookup<Entity> _idToEntity = new();

        private int _entityIdCounter = 0;

        public EntityFactory(World world)
        {
            _world = world;
        }

        public Entity Create(int parameters)
        {
            _idToEntity.TryGetValue(parameters, out var entity);
            if (entity == Entity.Null || !_world.IsAlive(entity))
                _idToEntity[parameters] = entity = _world.Create(new SyncId() { Value = parameters });

            _entityIdCounter = Maths.Max(parameters, _entityIdCounter);
            return entity;
        }

        public Entity Get(int entityId)
        {
            _idToEntity.TryGetValue(entityId, out var entity);
            return entity;
        }
        public int Reserve(int count = 1) => (_entityIdCounter += count);
    }
}
