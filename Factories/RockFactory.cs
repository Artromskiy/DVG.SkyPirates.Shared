using Arch.Core;
using DVG.SkyPirates.Shared.Archetypes;
using DVG.SkyPirates.Shared.Components.Special;
using DVG.SkyPirates.Shared.Entities;
using DVG.SkyPirates.Shared.Ids;
using DVG.SkyPirates.Shared.IFactories;

namespace DVG.SkyPirates.Shared.Factories
{
    public class RockFactory : IRockFactory
    {
        private readonly IEntityConfigFactory<RockId> _entityConfigFactory;
        private readonly World _world;

        public RockFactory(World world, IEntityConfigFactory<RockId> entityConfigFactory)
        {
            _entityConfigFactory = entityConfigFactory;
            _world = world;
        }

        public Entity Create((RockId RockId, int EntityId) parameters)
        {
            var config = _entityConfigFactory.Create(parameters.RockId);
            var entity = EntityIds.Get(parameters.EntityId);
            RockArch.EnsureArch(_world, entity);
            _world.SetConfig(entity, config);
            return entity;
        }
    }
}
