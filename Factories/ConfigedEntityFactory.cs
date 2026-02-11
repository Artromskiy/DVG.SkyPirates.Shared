using Arch.Core;
using DVG.Ids;
using DVG.SkyPirates.Shared.IFactories;
using DVG.SkyPirates.Shared.Systems.Special;
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

        public Entity Create((T Id, int SyncId) parameters)
        {
            var config = _entityConfigFactory.Create(parameters.Id);
            var entity = _entityFactory.Create(parameters.SyncId);
            _world.SetEntityData(entity, config);
            return _entityDependencyFactory.Create(entity);
        }
    }
}
