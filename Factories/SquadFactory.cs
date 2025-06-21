using DVG.Core;
using DVG.SkyPirates.Shared.Commands;
using DVG.SkyPirates.Shared.IFactories;
using DVG.SkyPirates.Shared.IServices;
using DVG.SkyPirates.Shared.Models;
using DVG.SkyPirates.Shared.Presenters;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Factories
{
    public class SquadFactory : ISquadFactory
    {
        private readonly IPathFactory<PackedCirclesModel> _circlesModelFactory;
        private readonly IEntitiesService _entitiesService;

        private readonly Dictionary<int, SquadPm> _squadsCache = new Dictionary<int, SquadPm>();

        public SquadFactory(IPathFactory<PackedCirclesModel> circlesModelFactory, IEntitiesService entitiesService)
        {
            _circlesModelFactory = circlesModelFactory;
            _entitiesService = entitiesService;
        }

        public SquadPm Create(Command<SpawnSquad> cmd)
        {
            if (_squadsCache.TryGetValue(cmd.EntityId, out var squad))
                return squad;

            return _squadsCache[cmd.EntityId] = new SquadPm(_circlesModelFactory, _entitiesService);
        }
    }
}
