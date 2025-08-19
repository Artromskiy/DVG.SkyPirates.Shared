using Arch.Core;
using Arch.Core.Extensions;
using DVG.Core;
using DVG.SkyPirates.Shared.Archetypes;
using DVG.SkyPirates.Shared.Commands;
using DVG.SkyPirates.Shared.Components;
using DVG.SkyPirates.Shared.Entities;
using DVG.SkyPirates.Shared.IFactories;
using System;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Factories
{
    public class SquadFactory : ISquadFactory
    {
        private readonly Dictionary<int, Entity> _squadsCache = new Dictionary<int, Entity>();

        public SquadFactory()
        {
        }

        public Entity Create(Command<SpawnSquadCommand> cmd)
        {
            if (_squadsCache.TryGetValue(cmd.EntityId, out var squad))
                return squad;

            _squadsCache[cmd.EntityId] = squad = EntityIds.Get(cmd.EntityId);

            SquadArch.EnsureArch(squad);
            HistoryArch.EnsureHistory(squad);
            squad.Get<Squad>().orders = new List<int>();
            squad.Get<Squad>().units = new List<Entity>();
            squad.Get<Squad>().positions = Array.Empty<fix2>();

            return squad;
        }
    }
}
