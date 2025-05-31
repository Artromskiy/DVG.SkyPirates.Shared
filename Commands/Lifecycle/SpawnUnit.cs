using DVG.Core;
using DVG.Core.Commands.Attributes;
using DVG.SkyPirates.Shared.Ids;
using System;

namespace DVG.SkyPirates.Shared.Commands.Lifecycle
{
    [Command]
    public readonly struct SpawnUnit: ICommandData
    {
        private readonly int _unitId;
        public readonly int level;
        public readonly int merge;

        public SpawnUnit(UnitId unitId, int level, int merge)
        {
            _unitId = Array.IndexOf(UnitId.Constants.AllIds, unitId);
            this.level = level;
            this.merge = merge;
        }

        public readonly UnitId UnitId => UnitId.Constants.AllIds[_unitId];

        public int CommandId => throw new NotImplementedException();
    }
}
