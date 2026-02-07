using Arch.Core;
using DVG.SkyPirates.Shared.Archetypes;
using DVG.SkyPirates.Shared.Ids;
using DVG.SkyPirates.Shared.IFactories;
using DVG.SkyPirates.Shared.Systems.Special;

namespace DVG.SkyPirates.Shared.Factories
{
    public class TreeFactory : ITreeFactory
    {
        private readonly IEntityFactory _commandEntityFactory;
        private readonly IEntityConfigFactory<TreeId> _entityConfigFactory;
        private readonly World _world;

        public TreeFactory(World world, IEntityConfigFactory<TreeId> entityConfigFactory, IEntityFactory commandEntityFactory)
        {
            _world = world;
            _entityConfigFactory = entityConfigFactory;
            _commandEntityFactory = commandEntityFactory;
        }

        public Entity Create((TreeId TreeId, int EntityId) parameters)
        {
            var config = _entityConfigFactory.Create(parameters.TreeId);
            var entity = _commandEntityFactory.Create(parameters.EntityId);
            TreeArch.EnsureArch(_world, entity);
            _world.SetEntityData(entity, config);
            return entity;
        }
    }
}
