using DVG.Core;
using DVG.SkyPirates.Shared.Commands;
using DVG.SkyPirates.Shared.Commands.Lifecycle;
using DVG.SkyPirates.Shared.IServices;
using DVG.SkyPirates.Shared.Models;
using DVG.SkyPirates.Shared.Presenters;

namespace DVG.SkyPirates.Shared.Services
{
    public class SpawnCommandsReciever
    {
        private readonly IPathFactory<PackedCirclesModel> _packedCirclesFactory;
        private readonly ICommandRecieveService _commandService;
        private readonly IInstanceIdsService _instanceIdsService;
        private readonly IPlayerLoopSystem _playerLoopSystem;

        public SpawnCommandsReciever(IPathFactory<PackedCirclesModel> packedCirclesFactory, ICommandRecieveService commandService, IInstanceIdsService instanceIdsService, IPlayerLoopSystem playerLoopSystem)
        {
            _packedCirclesFactory = packedCirclesFactory;
            _commandService = commandService;
            _instanceIdsService = instanceIdsService;
            _playerLoopSystem = playerLoopSystem;
            _commandService.RegisterReciever<SpawnSquad>(SpawnSquad);
        }

        public void SpawnSquad(Command<SpawnSquad> cmd, int callerId)
        {
            var squad = new SquadPm(_packedCirclesFactory);
            _instanceIdsService.AddInstance(cmd.InstanceId, squad);
            _playerLoopSystem.Add(squad);
        }

        public void DespawnSquad(Command<DespawnSquad> cmd, int callerId)
        {
            if (_instanceIdsService.RemoveInstance<IPlayerLoopItem>(cmd.InstanceId, out var instance))
                _playerLoopSystem.Remove(instance);
        }

        public void SpawnUnit(Command<SpawnUnit> cmd, int callerId)
        {

        }

        public void DespawnUnit(Command<DespawnUnit> cmd, int callerId)
        {

        }
    }
}
