using Arch.Core;
using DVG.Components;
using DVG.SkyPirates.Shared.Data;
using DVG.SkyPirates.Shared.IFactories;
using DVG.SkyPirates.Shared.IServices;

namespace DVG.SkyPirates.Shared.Factories
{
    public class EntityFactory : IEntityFactory
    {
        private readonly World _world;
        private readonly IEntityRegistry _entityRegistryService;

        public EntityFactory(World world, IEntityRegistry entityRegistryService)
        {
            _world = world;
            _entityRegistryService = entityRegistryService;
        }

        public Entity Create(EntityParameters parameters)
        {
            if (!_entityRegistryService.TryGet(parameters.SyncId, out var entity) ||
                entity == null || !_world.IsAlive(entity))
            {
                entity = _world.Create();
                _entityRegistryService.Register(entity, parameters.SyncId);
            }
            _world.AddOrGet<SyncId>(entity) = parameters.SyncId;
            _world.AddOrGet<SyncIdReserve>(entity) = parameters.SyncIdReserve;
            _world.AddOrGet<RandomSeed>(entity) = parameters.RandomSeed;
            return entity;
        }
    }
}
