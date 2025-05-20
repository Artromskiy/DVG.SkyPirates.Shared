using DVG.SkyPirates.Shared.Ids;
using System;

namespace DVG.SkyPirates.Shared.Commands
{
    public readonly partial struct RegisterSquadUnit
    {
        private readonly int _unitId;
        public readonly int level;
        public readonly int merge;

        public RegisterSquadUnit(UnitId unitId, int level, int merge)
        {
            _unitId = Array.IndexOf(UnitId.Constants.AllIds, unitId);
            this.level = level;
            this.merge = merge;
        }

        public readonly UnitId UnitId => UnitId.Constants.AllIds[_unitId];

    }
}
