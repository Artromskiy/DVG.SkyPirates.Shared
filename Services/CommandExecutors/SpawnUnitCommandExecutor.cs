using DVG.Core;
using DVG.SkyPirates.Shared.Commands;
using DVG.SkyPirates.Shared.IFactories;
using DVG.SkyPirates.Shared.IServices;
using DVG.SkyPirates.Shared.Entities;

namespace DVG.SkyPirates.Shared.Services.CommandExecutors
{
    public class SpawnUnitCommandExecutor :
        ICommandExecutor<SpawnUnitCommand>
    {
        private readonly IUnitFactory _unitFactory;
        private readonly IEntitiesService _entitiesService;
        private readonly IOwnershipService _ownershipService;

        public SpawnUnitCommandExecutor(
            IUnitFactory unitFactory,
            IEntitiesService entitiesService,
            IOwnershipService ownershipService)
        {
            _unitFactory = unitFactory;
            _entitiesService = entitiesService;
            _ownershipService = ownershipService;
        }

        public void Execute(Command<SpawnUnitCommand> cmd)
        {
            _entitiesService.TryGetEntity<SquadEntity>(cmd.Data.SquadId, out var squad);

            var unit = _unitFactory.Create(cmd);

            _entitiesService.AddEntity(cmd.EntityId, unit);
            _ownershipService.SetOwner(cmd.EntityId, cmd.ClientId);

            squad.AddUnit(cmd.EntityId);
        }
    }
}
