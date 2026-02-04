using Arch.Core;
using DVG.Core;
using DVG.Core.Components;
using DVG.SkyPirates.Shared.Archetypes;
using DVG.SkyPirates.Shared.Entities;
using DVG.SkyPirates.Shared.Ids;
using DVG.SkyPirates.Shared.IFactories;

namespace DVG.SkyPirates.Shared.Factories
{
    public class UnitFactory : IUnitFactory
    {
        private readonly IEntityConfigFactory _entityConfigFactory;
        private readonly World _world;

        public UnitFactory(World world, IEntityConfigFactory entityConfigFactory)
        {
            _world = world;
            _entityConfigFactory = entityConfigFactory;
        }

        public virtual Entity Create((UnitId UnitId, int EntityId) parameters)
        {
            var config = _entityConfigFactory.Create(parameters.UnitId);
            var entity = EntityIds.Get(parameters.EntityId);
            UnitArch.EnsureArch(_world, entity);
            Apply(config, _world, entity);
            _world.Get<UnitId>(entity) = parameters.UnitId;

            return entity;
        }


        public void Apply(Data.EntityData config, World world, Entity entity)
        {
            var action = new ApplyComponent(entity, world, config);
            ComponentIds.ForEachData(ref action);
        }

        private readonly struct ApplyComponent : IStructGenericAction
        {
            private readonly Entity _entity;
            private readonly World _world;
            private readonly Data.EntityData _config;

            public ApplyComponent(Entity entity, World world, Data.EntityData config)
            {
                _entity = entity;
                _world = world;
                _config = config;
            }

            public void Invoke<T>() where T : struct
            {
                var cmp = _config.Get<T>();
                if (cmp.HasValue && !_world.Has<T>(_entity))
                    _world.Add<T>(_entity);
                if (cmp.HasValue)
                    _world.Set(_entity, cmp.Value);
            }
        }
    }
}