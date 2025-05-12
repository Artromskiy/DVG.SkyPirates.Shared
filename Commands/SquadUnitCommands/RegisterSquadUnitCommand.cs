using DVG.SkyPirates.Shared.Ids;
using System;

namespace DVG.SkyPirates.Shared.Commands.SquadUnitCommands
{
    public readonly struct RegisterSquadUnitCommand
    {
        public readonly int squadId;

        private readonly int _unitId;
        public readonly int level;
        public readonly int merge;

        public RegisterSquadUnitCommand(int squadId, UnitId unitId, int level, int merge)
        {
            this.squadId = squadId;
            _unitId = Array.IndexOf(UnitId.Constants.AllIds, unitId);
            this.level = level;
            this.merge = merge;
        }
        public readonly UnitId UnitId => UnitId.Constants.AllIds[_unitId];
    }
}
