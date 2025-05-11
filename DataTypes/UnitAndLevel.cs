using DVG.SkyPirates.Shared.Ids;
using System;

namespace DVG.SkyPirates.Shared.DataTypes
{
    public readonly struct UnitAndLevel
    {
        private readonly int _unitId;
        public readonly int level;
        public readonly UnitId UnitId => UnitId.Constants.AllIds[_unitId];

        public UnitAndLevel(UnitId unitId, int level)
        {
            _unitId = Array.IndexOf(UnitId.Constants.AllIds, unitId);
            this.level = level;
        }
    }
}
