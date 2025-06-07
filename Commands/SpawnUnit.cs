using DVG.Core.Commands.Attributes;
using DVG.SkyPirates.Shared.Ids;
using System;

namespace DVG.SkyPirates.Shared.Commands
{
    [Command]
    public readonly partial struct SpawnUnit
    {
        private readonly int _unitId;
        public readonly int level;
        public readonly int merge;
        public readonly int squadEntityId;

        public SpawnUnit(UnitId unitId, int level, int merge, int squadEntityId)
        {
            _unitId = UnitId.Constants.AllIds.IndexOf(unitId);
            this.level = level;
            this.merge = merge;
            this.squadEntityId = squadEntityId;
        }

        public readonly UnitId UnitId => UnitId.Constants.AllIds[_unitId];
    }
}
