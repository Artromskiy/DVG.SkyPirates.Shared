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
        public int SquadId { get; set; }

        [IgnoreDataMember]
        public readonly UnitId UnitId => UnitId.Constants.AllIds[UnitIndex];

        public SpawnUnitCommand(int unitIndex, int squadId)
        {
            UnitIndex = unitIndex;
            SquadId = squadId;
        }

        public SpawnUnitCommand(UnitId unitId, int squadId) :
            this(UnitId.Constants.AllIds.IndexOf(unitId), squadId)
        { }
    }
}
