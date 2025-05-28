using DVG.Core;
using DVG.SkyPirates.Shared.Commands;
using DVG.SkyPirates.Shared.Commands.Lifecycle;
using DVG.SkyPirates.Shared.IServices;
using DVG.SkyPirates.Shared.Presenters;

namespace DVG.SkyPirates.Shared.Services
{
    public class SpawnCommandsReciever
    {
        private readonly IFactory<SquadPm, Command<SpawnSquad>> _squadFactory;
        private readonly IFactory<UnitPm, Command<SpawnUnit>> _unitFactory;
        private readonly ICommandRecieveService _commandService;
        private readonly IInstanceIdsService _instanceIdsService;

        public SpawnCommandsReciever(
            IFactory<SquadPm, Command<SpawnSquad>> squadFactory,
            IFactory<UnitPm, Command<SpawnUnit>> unitFactory,
            ICommandRecieveService commandService,
            IInstanceIdsService instanceIdsService)
        {
            _squadFactory = squadFactory;
            _unitFactory = unitFactory;

            _commandService = commandService;
            _instanceIdsService = instanceIdsService;

            _commandService.RegisterReciever<SpawnSquad>(SpawnSquad);
            _commandService.RegisterReciever<SpawnUnit>(SpawnUnit);

            _commandService.RegisterReciever<DespawnSquad>(DespawnSquad);
            _commandService.RegisterReciever<DespawnUnit>(DespawnUnit);
        }

        public void SpawnSquad(Command<SpawnSquad> cmd)
        {
            var squad = _squadFactory.Create(cmd);
            _instanceIdsService.AddInstance(cmd.InstanceId, squad);
        }

        public void DespawnSquad(Command<DespawnSquad> cmd)
        {
            if (_instanceIdsService.RemoveInstance<SquadPm>(cmd.InstanceId, out var instance))
                _squadFactory.Dispose(instance);
        }

        public void SpawnUnit(Command<SpawnUnit> cmd)
        {
            var unit = _unitFactory.Create(cmd);
            _instanceIdsService.AddInstance(cmd.InstanceId, unit);
        }

        public void DespawnUnit(Command<DespawnUnit> cmd)
        {
            if (_instanceIdsService.RemoveInstance<UnitPm>(cmd.InstanceId, out var instance))
                _unitFactory.Dispose(instance);
        }
    }
}
