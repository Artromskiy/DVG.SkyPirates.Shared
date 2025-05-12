using DVG.Core;
using DVG.SkyPirates.Shared.Commands.SquadCommands;
using DVG.SkyPirates.Shared.Commands.SquadUnitCommands;
using DVG.SkyPirates.Shared.IFactories;
using DVG.SkyPirates.Shared.IServices;
using DVG.SkyPirates.Shared.Models;
using DVG.SkyPirates.Shared.Presenters;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Services
{
    internal class SquadCommandsReciever : ITickable
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

            _commandService.RegisterReciever<RegisterSquadUnitCommand>(RegisterSquadUnit);
            _commandService.RegisterReciever<UnregisterSquadUnitCommand>(UnregisterSquadUnit);

            _commandService.RegisterReciever<RegisterSquadCommand>(RegisterSquad);
            _commandService.RegisterReciever<MoveSquadCommand>(MoveSquad);
            _commandService.RegisterReciever<RotateSquadCommand>(RotateSquad);
            _commandService.RegisterReciever<FixateSquadCommand>(FixateSquad);
        }

        public void RegisterSquadUnit(RegisterSquadUnitCommand cmd, int callerId)
        {
            if (_squads.TryGetValue(cmd.squadId, out var squad))
            {
                var unit = _unitFactory.Create((cmd.UnitId, cmd.level, cmd.merge));
                squad.AddUnit(unit, squad.UnitsCount);
            }
        }

        public void UnregisterSquadUnit(UnregisterSquadUnitCommand cmd, int callerId)
        {
            if (_squads.TryGetValue(cmd.squadId, out var squad))
            {
                squad.RemoveUnit(cmd.unitId);
            }
        }

        public void ReorderSquadUnits(UnregisterSquadUnitCommand cmd, int callerId) { }

        public void RegisterSquad(RegisterSquadCommand cmd, int callerId)
        {
            if (_squads.ContainsKey(cmd.squadId))
                return;
            var squad = new SquadPm(_packedCirclesFactory);
            _squads.Add(cmd.squadId, squad);
        }

        public void MoveSquad(MoveSquadCommand cmd, int callerId)
        {
            if (_squads.TryGetValue(cmd.squadId, out var squad))
            {
                squad.Position = cmd.position;
            }
        }

        public void RotateSquad(RotateSquadCommand cmd, int callerId)
        {
            if (_squads.TryGetValue(cmd.squadId, out var squad))
            {
                squad.Rotation = cmd.rotation;
            }
        }

        public void FixateSquad(FixateSquadCommand cmd, int callerId)
        {
            if (_squads.TryGetValue(cmd.squadId, out var squad))
            {
                squad.Fixation = cmd.fixation;
            }
        }

        public void Tick()
        {
            foreach (var item in _squads)
                item.Value.Tick();
        }
    }
}
