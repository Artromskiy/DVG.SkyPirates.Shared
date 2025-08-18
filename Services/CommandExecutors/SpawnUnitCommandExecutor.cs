using DVG.Core;
using DVG.SkyPirates.Shared.Commands;
using DVG.SkyPirates.Shared.Entities;
using DVG.SkyPirates.Shared.IFactories;
using DVG.SkyPirates.Shared.IServices;

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
            var squad = _entitiesService.GetEntity<SquadEntity>(cmd.Data.SquadId);
            var unit = _unitFactory.Create(cmd);
            unit.Position = squad.Position;
            _entitiesService.AddEntity(cmd.EntityId, unit);
            _ownershipService.SetOwner(cmd.EntityId, cmd.ClientId);

            squad.AddUnit(cmd.EntityId);
        }
    }
}
