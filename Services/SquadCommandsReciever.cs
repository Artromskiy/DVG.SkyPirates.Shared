using DVG.Core;
using DVG.SkyPirates.Shared.Commands;
using DVG.SkyPirates.Shared.IFactories;
using DVG.SkyPirates.Shared.IServices;
using DVG.SkyPirates.Shared.Models;
using DVG.SkyPirates.Shared.Presenters;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Services
{
    public class SquadCommandsReciever : ITickable
    {
        private readonly Dictionary<int, SquadPm> _squads = new Dictionary<int, SquadPm>();

        private readonly ICommandRecieveService _commandService;
        private readonly IPathFactory<PackedCirclesModel> _packedCirclesFactory;
        private readonly IUnitFactory _unitFactory;

        public SquadCommandsReciever(ICommandRecieveService commandService, IPathFactory<PackedCirclesModel> packedCirclesFactory, IUnitFactory unitFactory)
        {
            _commandService = commandService;
            _packedCirclesFactory = packedCirclesFactory;
            _unitFactory = unitFactory;

            _commandService.RegisterReciever<RegisterSquadUnit>(RegisterSquadUnit);
            _commandService.RegisterReciever<UnregisterSquadUnit>(UnregisterSquadUnit);

            _commandService.RegisterReciever<RegisterSquad>(RegisterSquad);
            _commandService.RegisterReciever<MoveSquad>(MoveSquad);
            _commandService.RegisterReciever<RotateSquad>(RotateSquad);
            _commandService.RegisterReciever<FixateSquad>(FixateSquad);
        }

        public void RegisterSquadUnit(Command<RegisterSquadUnit> cmd, int callerId)
        {
            if (_squads.TryGetValue(cmd.callerId, out var squad))
            {
                var unit = _unitFactory.Create((cmd.data.UnitId, cmd.data.level, cmd.data.merge));
                squad.AddUnit(unit, squad.UnitsCount);
            }
        }

        public void UnregisterSquadUnit(Command<UnregisterSquadUnit> cmd, int callerId)
        {
            if (_squads.TryGetValue(cmd.callerId, out var squad))
            {
                squad.RemoveUnit(cmd.data.unitId);
            }
        }

        public void ReorderSquadUnits(Command<UnregisterSquadUnit> cmd, int callerId) { }

        public void RegisterSquad(Command<RegisterSquad> cmd, int callerId)
        {
            if (_squads.ContainsKey(cmd.callerId))
                return;
            var squad = new SquadPm(_packedCirclesFactory);
            _squads.Add(cmd.callerId, squad);
        }

        public void MoveSquad(Command<MoveSquad> cmd, int callerId)
        {
            if (_squads.TryGetValue(cmd.callerId, out var squad))
            {
                squad.Position = cmd.data.position;
            }
        }

        public void RotateSquad(Command<RotateSquad> cmd, int callerId)
        {
            if (_squads.TryGetValue(cmd.callerId, out var squad))
            {
                squad.Rotate(cmd.data.rotation);
            }
        }

        public void FixateSquad(Command<FixateSquad> cmd, int callerId)
        {
            if (_squads.TryGetValue(cmd.callerId, out var squad))
            {
                squad.Fixation = cmd.data.fixation;
            }
        }

        public void Tick(float deltaTime)
        {
            foreach (var item in _squads)
                item.Value.Tick(deltaTime);
        }
    }
}