using Arch.Core;
using DVG.SkyPirates.Shared.Components.Special;
using DVG.SkyPirates.Shared.IFactories;
using System;

namespace DVG.SkyPirates.Shared.Factories
{
    public class EntityFactory : IEntityFactory
    {
        private readonly World _world;

        private Entity[] _idToEntity = Array.Empty<Entity>();

        private int _entityIdCounter = 0;

        public EntityFactory(World world)
        {
            _world = world;
        }

        public Entity Create(int parameters)
        {
            if (_idToEntity.Length <= parameters)
            {
                Array.Resize(ref _idToEntity, (parameters + 1) << 1);
            }

            var entity = _idToEntity[parameters];
            if (entity == Entity.Null || !_world.IsAlive(entity))
                _idToEntity[parameters] = entity = _world.Create(new SyncId() { Value = parameters });

            return entity;
        }

        public Entity Get(int entityId)
        {
            if (_idToEntity.Length <= entityId)
                return Entity.Null;

            return _idToEntity[entityId];
        }

        public int Next() => _entityIdCounter++;
    }
}
