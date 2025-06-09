using DVG.Core.Commands.Attributes;
using DVG.SkyPirates.Shared.Ids;
using System;
using System.Runtime.Serialization;

namespace DVG.SkyPirates.Shared.Commands
{
    [Command]
    [DataContract]
    public readonly partial struct SpawnUnit
    {
        [DataMember(Order = 0)]
        private readonly int _unitId;
        [DataMember(Order = 1)]
        public readonly int level;
        [DataMember(Order = 2)]
        public readonly int merge;
        [DataMember(Order = 3)]
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
