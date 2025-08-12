using DVG.Core;
using DVG.SkyPirates.Shared.Commands;
using DVG.SkyPirates.Shared.IFactories;
using DVG.SkyPirates.Shared.IServices;
using DVG.SkyPirates.Shared.Configs;
using DVG.SkyPirates.Shared.Entities;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Factories
{
    public class SquadFactory : ISquadFactory
    {
        private readonly IPathFactory<PackedCirclesConfig> _circlesModelFactory;
        private readonly IEntitiesService _entitiesService;

        private readonly Dictionary<int, SquadEntity> _squadsCache = new Dictionary<int, SquadEntity>();

        public SquadFactory(IPathFactory<PackedCirclesConfig> circlesModelFactory, IEntitiesService entitiesService)
        {
            _circlesModelFactory = circlesModelFactory;
            _entitiesService = entitiesService;
        }

        public SquadEntity Create(Command<SpawnSquad> cmd)
        {
            if (_squadsCache.TryGetValue(cmd.EntityId, out var squad))
                return squad;

            return _squadsCache[cmd.EntityId] = new SquadEntity(_circlesModelFactory, _entitiesService);
        }
    }
}
