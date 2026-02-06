using Arch.Core;
using DVG.Core;
using DVG.SkyPirates.Shared.Archetypes;
using DVG.SkyPirates.Shared.Commands;
using DVG.SkyPirates.Shared.Components;
using DVG.SkyPirates.Shared.Components.Config;
using DVG.SkyPirates.Shared.Components.Runtime;
using DVG.SkyPirates.Shared.IFactories;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Factories
{
    public class SquadFactory : ISquadFactory
    {
        private readonly ICommandEntityFactory _commandEntityFactory;
        private readonly World _world;
        public SquadFactory(World world, ICommandEntityFactory commandEntityFactory)
        {
            _world = world;
            _commandEntityFactory = commandEntityFactory;
        }

        public Entity Create(Command<SpawnSquadCommand> cmd)
        {
            var entity = _commandEntityFactory.Create(cmd.EntityId);

            SquadArch.EnsureArch(_world, entity);

            _world.Get<Radius>(entity).Value = fix.One / 3;
            _world.Get<Squad>(entity).units = new List<Entity>();
            _world.Get<Team>(entity).Id = cmd.ClientId;
            return entity;
        }
    }
}
