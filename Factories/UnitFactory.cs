using Arch.Core;
using DVG.SkyPirates.Shared.Archetypes;
using DVG.SkyPirates.Shared.Ids;
using DVG.SkyPirates.Shared.IFactories;
using DVG.SkyPirates.Shared.Systems.Special;

namespace DVG.SkyPirates.Shared.Factories
{
    public class UnitFactory : IUnitFactory
    {
        private readonly IEntityFactory _commandEntityFactory;
        private readonly IEntityConfigFactory<UnitId> _entityConfigFactory;
        private readonly World _world;

        public UnitFactory(World world, IEntityConfigFactory<UnitId> entityConfigFactory, IEntityFactory commandEntityFactory)
        {
            _world = world;
            _entityConfigFactory = entityConfigFactory;
            _commandEntityFactory = commandEntityFactory;
        }

        public virtual Entity Create((UnitId UnitId, int EntityId) parameters)
        {
            var config = _entityConfigFactory.Create(parameters.UnitId);
            var entity = _commandEntityFactory.Create(parameters.EntityId);
            UnitArch.EnsureArch(_world, entity);
            _world.SetEntityData(entity, config);

            return entity;
        }
    }
}