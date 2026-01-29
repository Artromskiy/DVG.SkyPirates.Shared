using Arch.Core;
using DVG.SkyPirates.Shared.Archetypes;
using DVG.SkyPirates.Shared.Components;
using DVG.SkyPirates.Shared.Components.Data;
using DVG.SkyPirates.Shared.Entities;
using DVG.SkyPirates.Shared.Ids;
using DVG.SkyPirates.Shared.IFactories;

namespace DVG.SkyPirates.Shared.Factories
{
    public class TreeFactory : ITreeFactory
    {
        private readonly World _world;

        public TreeFactory(World world)
        {
            _world = world;
        }

        public Entity Create((TreeId TreeId, int EntityId) parameters)
        {
            var entity = EntityIds.Get(parameters.EntityId);
            TreeArch.EnsureArch(_world, entity);

            _world.Get<TreeId>(entity) = parameters.TreeId;
            _world.Get<Health>(entity).Value = 100;
            _world.Get<MaxHealth>(entity).Value = 100;
            _world.Get<CircleShape>(entity).Radius = fix.One / 2;
            _world.Get<Separation>(entity).AddRadius = fix.One / 2;
            _world.Get<Separation>(entity).AffectingCoeff = 1;
            _world.Get<Separation>(entity).AffectedCoeff = 0;
            _world.Get<AutoHeal>(entity).healDelay = 10;
            _world.Get<AutoHeal>(entity).healPerSecond = 20;
            return entity;
        }
    }
}
