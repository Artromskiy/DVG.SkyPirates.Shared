using Arch.Core;
using DVG.SkyPirates.Shared.Data;
using DVG.SkyPirates.Shared.IFactories;
using DVG.SkyPirates.Shared.IServices;

namespace DVG.SkyPirates.Shared.Factories
{
    public class EntityFactory : IEntityFactory
    {
        private readonly World _world;
        private readonly IEntityRegistryService _entityRegistryService;

        public EntityFactory(World world, IEntityRegistryService entityRegistryService)
        {
            _world = world;
            _entityRegistryService = entityRegistryService;
        }

        public Entity Create(EntityParameters parameters)
        {
            if (!_entityRegistryService.TryGet(parameters.SyncId, out var entity) ||
                entity == null || !_world.IsAlive(entity))
            {
                entity = _world.Create(parameters.SyncId, parameters.SyncIdReserve, parameters.RandomSeed);
                _entityRegistryService.Register(entity, parameters.SyncId);
            }
            return entity;
        }
    }
}
