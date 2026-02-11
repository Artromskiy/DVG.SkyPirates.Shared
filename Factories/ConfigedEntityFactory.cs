using Arch.Core;
using DVG.Ids;
using DVG.SkyPirates.Shared.Data;
using DVG.SkyPirates.Shared.IFactories;
using DVG.SkyPirates.Shared.Systems;
using System;

namespace DVG.SkyPirates.Shared.Factories
{
    public class ConfigedEntityFactory<T> : IConfigedEntityFactory<T>
        where T : struct, IId, IEquatable<T>
    {
        private readonly IEntityFactory _entityFactory;
        private readonly IEntityConfigFactory<T> _entityConfigFactory;
        private readonly IEntityDependencyFactory _entityDependencyFactory;
        private readonly World _world;

        public ConfigedEntityFactory(IEntityFactory entityFactory, IEntityConfigFactory<T> entityConfigFactory, IEntityDependencyFactory entityDependencyFactory, World world)
        {
            _entityFactory = entityFactory;
            _entityConfigFactory = entityConfigFactory;
            _entityDependencyFactory = entityDependencyFactory;
            _world = world;
        }

        public Entity Create((T Id, EntityParameters parameters) parameters)
        {
            var config = _entityConfigFactory.Create(parameters.Id);
            var entity = _entityFactory.Create(parameters.parameters);
            _world.SetEntityData(entity, config);
            return _entityDependencyFactory.Create(entity);
        }
    }
}
