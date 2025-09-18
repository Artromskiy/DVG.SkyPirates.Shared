using DVG.SkyPirates.Shared.Ids;
using System;
using System.Runtime.Serialization;

namespace DVG.SkyPirates.Shared.Data
{
    [Serializable]
    [DataContract]
    public partial class UnitAndLevel
    {
        [DataMember(Order = 0)]
        public UnitId unitId;
        [DataMember(Order = 1)]
        public int level;

        public UnitAndLevel(UnitId unitId, int level)
        {
            this.unitId = unitId;
            this.level = level;
        }
    }
}
