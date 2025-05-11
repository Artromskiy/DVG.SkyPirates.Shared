using DVG.SkyPirates.Shared.Ids;
using System;

namespace DVG.SkyPirates.Shared.Commands
{
    public struct RegisterSquadUnitCommand
    {
        public int id;

        public int unitId;
        public int level;
        public int merge;

        public RegisterSquadUnitCommand(int id, UnitId unitId, int level, int merge)
        {
            this.id = id;
            this.unitId = Array.IndexOf(UnitId.Constants.AllIds, unitId);
            this.level = level;
            this.merge = merge;
        }
    }
}
