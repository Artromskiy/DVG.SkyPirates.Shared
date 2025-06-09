using DVG.Core;
using DVG.SkyPirates.Shared.Commands;
using DVG.SkyPirates.Shared.IFactories;
using DVG.SkyPirates.Shared.Models;
using DVG.SkyPirates.Shared.Presenters;

namespace DVG.SkyPirates.Shared.Factories
{
    public class SquadFactory : ISquadFactory
    {
        private readonly IPathFactory<PackedCirclesModel> _circlesModelFactory;

        public SquadFactory(IPathFactory<PackedCirclesModel> circlesModelFactory)
        {
            _circlesModelFactory = circlesModelFactory;
        }

        public SquadPm Create(Command<SpawnSquad> parameters)
        {
            var squad = new SquadPm(_circlesModelFactory);
            return squad;
        }

        public void Dispose(SquadPm instance) { }
    }
}
