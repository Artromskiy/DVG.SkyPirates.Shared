using DVG.Core;
using DVG.SkyPirates.Shared.Commands;
using DVG.SkyPirates.Shared.IFactories;
using DVG.SkyPirates.Shared.IServices;
using DVG.SkyPirates.Shared.Presenters;

namespace DVG.SkyPirates.Shared.Services.CommandExecutors
{
    public class SpawnUnitCommandExecutor :
        ICommandExecutor<SpawnUnit>
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

        public void Execute(Command<SpawnUnit> cmd)
        {
            _entitiesService.TryGetEntity<SquadPm>(cmd.Data.squadEntityId, out var squad);
            var unit = _unitFactory.Create(cmd.Data);
            _entitiesService.AddEntity(cmd.EntityId, unit);
            _ownershipService.SetOwner(cmd.EntityId, cmd.ClientId);
            squad.AddUnit(cmd.EntityId);
        }
    }
}
