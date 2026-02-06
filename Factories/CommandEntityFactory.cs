using Arch.Core;
using DVG.SkyPirates.Shared.IFactories;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Factories
{
    public class CommandEntityFactory : ICommandEntityFactory
    {
        private readonly World _world;
        private readonly Dictionary<int, Entity> _idToEntity = new();
        private readonly Dictionary<Entity, int> _entityToIdMapping = new();

        private int _entityIdCounter = 0;

        public CommandEntityFactory(World world)
        {
            _world = world;
        }

        public Entity Create(int parameters)
        {
            if (!_idToEntity.TryGetValue(parameters, out var entity) || !_world.IsAlive(entity))
                _idToEntity[parameters] = entity = _world.Create();

            _entityToIdMapping[entity] = parameters;
            return entity;
        }

        public Entity Get(int entityId)
        {
            if (!_idToEntity.TryGetValue(entityId, out var entity) || !_world.IsAlive(entity))
                return Entity.Null;
            return entity;
        }

        public int Get(Entity entity) => _entityToIdMapping[entity];

        public int Next() => _entityIdCounter++;
    }
}
