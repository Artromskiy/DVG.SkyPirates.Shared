using DVG.Core.Commands.Attributes;
using DVG.SkyPirates.Shared.Ids;
using System;
using System.Runtime.Serialization;

namespace DVG.SkyPirates.Shared.Commands
{
    [Command]
    [DataContract]
    public partial struct SpawnUnitCommand
    {
        [DataMember(Order = 0)]
        public int UnitIndex { get; set; }
        [DataMember(Order = 1)]
        public int Level { get; set; }
        [DataMember(Order = 2)]
        public int Merge { get; set; }
        [DataMember(Order = 3)]
        public int SquadId { get; set; }

        [IgnoreDataMember]
        public readonly UnitId UnitId => UnitId.Constants.AllIds[UnitIndex];

        public SpawnUnitCommand(int unitIndex, int level, int merge, int squadEntityId)
        {
            UnitIndex = unitIndex;
            Level = level;
            Merge = merge;
            SquadId = squadEntityId;
        }

        public SpawnUnitCommand(UnitId unitId, int level, int merge, int squadEntityId) :
            this(UnitId.Constants.AllIds.IndexOf(unitId), level, merge, squadEntityId)
        { }
    }
}
