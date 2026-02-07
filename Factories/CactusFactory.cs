using Arch.Core;
using DVG.SkyPirates.Shared.Archetypes;
using DVG.SkyPirates.Shared.Components.Config;
using DVG.SkyPirates.Shared.Ids;
using DVG.SkyPirates.Shared.IFactories;
using DVG.SkyPirates.Shared.Systems.Special;

namespace DVG.SkyPirates.Shared.Factories
{
    public class CactusFactory : ICactusFactory
    {
        private readonly IEntityFactory _commandEntityFactory;
        private readonly IEntityConfigFactory<CactusId> _entityConfigFactory;
        private readonly World _world;

        public CactusFactory(World world, IEntityConfigFactory<CactusId> entityConfigFactory, IEntityFactory commandEntityFactory)
        {
            _world = world;
            _entityConfigFactory = entityConfigFactory;
            _commandEntityFactory = commandEntityFactory;
        }

        public Entity Create((CactusId CactusId, int EntityId) parameters)
        {
            var config = _entityConfigFactory.Create(parameters.CactusId);
            var entity = _commandEntityFactory.Create(parameters.EntityId);
            CactusArch.EnsureArch(_world, entity);
            _world.SetEntityData(entity, config);

            _world.Get<Damage>(entity).Value = 1;
            _world.Get<ImpactDistance>(entity).Value = 1;
            _world.Get<TargetSearchDistance>(entity).Value = 1;

            return entity;
        }
    }
}
