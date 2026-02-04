using Arch.Core;
using DVG.SkyPirates.Shared.Archetypes;
using DVG.SkyPirates.Shared.Components;
using DVG.SkyPirates.Shared.Components.Config;
using DVG.SkyPirates.Shared.Entities;
using DVG.SkyPirates.Shared.Ids;
using DVG.SkyPirates.Shared.IFactories;

namespace DVG.SkyPirates.Shared.Factories
{
    public class RockFactory : IRockFactory
    {
        private readonly World _world;

        public RockFactory(World world)
        {
            _world = world;
        }

        public Entity Create((RockId RockId, int EntityId) parameters)
        {
            var entity = EntityIds.Get(parameters.EntityId);
            RockArch.EnsureArch(_world, entity);

            _world.Get<RockId>(entity) = parameters.RockId;
            _world.Get<MaxHealth>(entity).Value = 100;
            _world.Get<Radius>(entity).Value = fix.One / 3;
            _world.Get<Separation>(entity).AddRadius = fix.One / 3;
            _world.Get<Separation>(entity).AffectingCoeff = 1;
            _world.Get<Separation>(entity).AffectedCoeff = 0;
            return entity;
        }
    }
}
