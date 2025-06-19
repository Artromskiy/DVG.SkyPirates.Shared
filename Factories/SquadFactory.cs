using DVG.Core;
using DVG.SkyPirates.Shared.Commands;
using DVG.SkyPirates.Shared.IFactories;
using DVG.SkyPirates.Shared.IServices;
using DVG.SkyPirates.Shared.Models;
using DVG.SkyPirates.Shared.Presenters;

namespace DVG.SkyPirates.Shared.Factories
{
    public class SquadFactory : ISquadFactory
    {
        private readonly IPathFactory<PackedCirclesModel> _circlesModelFactory;
        private readonly IEntitiesService _entitiesService;

        public SquadFactory(IPathFactory<PackedCirclesModel> circlesModelFactory, IEntitiesService entitiesService)
        {
            _circlesModelFactory = circlesModelFactory;
            _entitiesService = entitiesService;
        }

        public SquadPm Create(Command<SpawnSquad> parameters)
        {
            var squad = new SquadPm(_circlesModelFactory, _entitiesService);
            return squad;
        }

        public void Dispose(SquadPm instance) { }
    }
}
