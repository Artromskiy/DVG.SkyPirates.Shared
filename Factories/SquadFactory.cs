using Arch.Core;
using DVG.Core;
using DVG.SkyPirates.Shared.Archetypes;
using DVG.SkyPirates.Shared.Commands;
using DVG.SkyPirates.Shared.Components;
using DVG.SkyPirates.Shared.Components.Data;
using DVG.SkyPirates.Shared.Entities;
using DVG.SkyPirates.Shared.IFactories;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Factories
{
    public class SquadFactory : ISquadFactory
    {
        private readonly World _world;
        public SquadFactory(World world)
        {
            _world = world;
        }

        public Entity Create(Command<SpawnSquadCommand> cmd)
        {
            var squad = EntityIds.Get(cmd.EntityId);

            SquadArch.EnsureArch(_world, squad);

            _world.Get<CircleShape>(squad).Radius = fix.One / 3;
            _world.Get<Squad>(squad).units = new List<Entity>();
            return squad;
        }
    }
}
