using Arch.Core;
using DVG.SkyPirates.Shared.Archetypes;
using DVG.SkyPirates.Shared.Components.Special;
using DVG.SkyPirates.Shared.Entities;
using DVG.SkyPirates.Shared.Ids;
using DVG.SkyPirates.Shared.IFactories;

namespace DVG.SkyPirates.Shared.Factories
{
    public class UnitFactory : IUnitFactory
    {
        private readonly IEntityConfigFactory<UnitId> _entityConfigFactory;
        private readonly World _world;

        public UnitFactory(World world, IEntityConfigFactory<UnitId> entityConfigFactory)
        {
            _world = world;
            _entityConfigFactory = entityConfigFactory;
        }

        public virtual Entity Create((UnitId UnitId, int EntityId) parameters)
        {
            var config = _entityConfigFactory.Create(parameters.UnitId);
            var entity = EntityIds.Get(parameters.EntityId);
            UnitArch.EnsureArch(_world, entity);
            _world.SetConfig(entity, config);
            _world.Get<UnitId>(entity) = parameters.UnitId;

            return entity;
        }
    }
}