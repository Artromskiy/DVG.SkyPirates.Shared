using Arch.Core;
using DVG.SkyPirates.Shared.Archetypes;
using DVG.SkyPirates.Shared.Components;
using DVG.SkyPirates.Shared.Components.Config;
using DVG.SkyPirates.Shared.Components.Special;
using DVG.SkyPirates.Shared.Entities;
using DVG.SkyPirates.Shared.Ids;
using DVG.SkyPirates.Shared.IFactories;

namespace DVG.SkyPirates.Shared.Factories
{
    public class CactusFactory : ICactusFactory
    {
        private readonly IEntityConfigFactory<CactusId> _entityConfigFactory;
        private readonly World _world;

        public CactusFactory(World world, IEntityConfigFactory<CactusId> entityConfigFactory)
        {
            _entityConfigFactory = entityConfigFactory;
            _world = world;
        }

        public Entity Create((CactusId CactusId, int EntityId) parameters)
        {
            var config = _entityConfigFactory.Create(parameters.CactusId);
            var entity = EntityIds.Get(parameters.EntityId);
            CactusArch.EnsureArch(_world, entity);
            _world.SetConfig(entity, config);

            _world.Get<Damage>(entity).Value = 1;
            _world.Get<ImpactDistance>(entity).Value = 1;
            _world.Get<TargetSearchDistance>(entity).Value = 1;

            return entity;
        }
    }
}
