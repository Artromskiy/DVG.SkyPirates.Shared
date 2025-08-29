using Arch.Core;
using Arch.Core.Extensions;
using DVG.Core;
using DVG.SkyPirates.Shared.Archetypes;
using DVG.SkyPirates.Shared.Commands;
using DVG.SkyPirates.Shared.Components;
using DVG.SkyPirates.Shared.Components.Special;
using DVG.SkyPirates.Shared.Entities;
using DVG.SkyPirates.Shared.IFactories;
using System;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Factories
{
    public class SquadFactory : ISquadFactory
    {
        public SquadFactory()
        {
        }

        public Entity Create(Command<SpawnSquadCommand> cmd)
        {
            var squad = EntityIds.Get(cmd.EntityId);

            SquadArch.EnsureArch(squad);
            HistoryArch.EnsureHistory(squad);
            squad.Get<Squad>().units = new List<Entity>();
            return squad;
        }
    }
}
