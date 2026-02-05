using Arch.Core;
using DVG.SkyPirates.Shared.Archetypes;
using DVG.SkyPirates.Shared.Components.Special;
using DVG.SkyPirates.Shared.Entities;
using DVG.SkyPirates.Shared.Ids;
using DVG.SkyPirates.Shared.IFactories;

namespace DVG.SkyPirates.Shared.Factories
{
    public class TreeFactory : ITreeFactory
    {
        private readonly IEntityConfigFactory<TreeId> _entityConfigFactory;
        private readonly World _world;

        public TreeFactory(World world, IEntityConfigFactory<TreeId> entityConfigFactory)
        {
            _world = world;
            _entityConfigFactory = entityConfigFactory;
        }

        public Entity Create((TreeId TreeId, int EntityId) parameters)
        {
            var config = _entityConfigFactory.Create(parameters.TreeId);
            var entity = EntityIds.Get(parameters.EntityId);
            TreeArch.EnsureArch(_world, entity);
            _world.SetConfig(entity, config);
            return entity;
        }
    }
}
