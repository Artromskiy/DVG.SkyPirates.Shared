using DVG.Core;
using DVG.SkyPirates.Shared.Commands;
using DVG.SkyPirates.Shared.IFactories;
using DVG.SkyPirates.Shared.IServices;
using DVG.SkyPirates.Shared.Configs;
using DVG.SkyPirates.Shared.Entities;
using System.Collections.Generic;
using Arch.Core;
using DVG.SkyPirates.Shared.Components;
using Arch.Core.Extensions;

namespace DVG.SkyPirates.Shared.Factories
{
    public class SquadFactory : ISquadFactory
    {
        private readonly IPathFactory<PackedCirclesConfig> _circlesModelFactory;

        private readonly Dictionary<int, Entity> _squadsCache = new Dictionary<int, Entity>();

        public SquadFactory(IPathFactory<PackedCirclesConfig> circlesModelFactory)
        {
            _circlesModelFactory = circlesModelFactory;
        }

        public Entity Create(Command<SpawnSquadCommand> cmd)
        {
            if (_squadsCache.TryGetValue(cmd.EntityId, out var squad))
                return squad;

            _squadsCache[cmd.EntityId] = squad = EntityIds.Get(cmd.EntityId);

            squad.Add<
                Squad, 
                Position, 
                Rotation, 
                Fixation, 
                Direction>();

            return squad;
        }
    }
}
