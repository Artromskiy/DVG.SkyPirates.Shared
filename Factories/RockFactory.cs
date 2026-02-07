using Arch.Core;
using DVG.SkyPirates.Shared.Archetypes;
using DVG.SkyPirates.Shared.Ids;
using DVG.SkyPirates.Shared.IFactories;
using DVG.SkyPirates.Shared.Systems.Special;

namespace DVG.SkyPirates.Shared.Factories
{
    public class RockFactory : IRockFactory
    {
        private readonly IEntityFactory _commandEntityFactory;
        private readonly IEntityConfigFactory<RockId> _entityConfigFactory;
        private readonly World _world;

        public RockFactory(World world, IEntityConfigFactory<RockId> entityConfigFactory, IEntityFactory commandEntityFactory)
        {
            _world = world;
            _entityConfigFactory = entityConfigFactory;
            _commandEntityFactory = commandEntityFactory;
        }

        public Entity Create((RockId RockId, int EntityId) parameters)
        {
            var config = _entityConfigFactory.Create(parameters.RockId);
            var entity = _commandEntityFactory.Create(parameters.EntityId);
            RockArch.EnsureArch(_world, entity);
            _world.SetEntityData(entity, config);
            return entity;
        }
    }
}
