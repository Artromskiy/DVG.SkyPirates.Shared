using DVG.Core;
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
        private readonly IEntitiesService _instanceIdsService;

        public SpawnCommandsReciever(
            IFactory<SquadPm, Command<SpawnSquad>> squadFactory,
            IFactory<UnitPm, Command<SpawnUnit>> unitFactory,
            ICommandRecieveService commandService,
            IEntitiesService instanceIdsService)
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
            _instanceIdsService.AddEntity(cmd.EntityId, squad);
        }

        public void DespawnSquad(Command<DespawnSquad> cmd)
        {
            if (_instanceIdsService.RemoveEntity<SquadPm>(cmd.EntityId, out var instance))
                _squadFactory.Dispose(instance);
        }

        public void SpawnUnit(Command<SpawnUnit> cmd)
        {
            var unit = _unitFactory.Create(cmd);
            _instanceIdsService.AddEntity(cmd.EntityId, unit);
        }

        public void DespawnUnit(Command<DespawnUnit> cmd)
        {
            if (_instanceIdsService.RemoveEntity<UnitPm>(cmd.EntityId, out var instance))
                _unitFactory.Dispose(instance);
        }
    }
}
