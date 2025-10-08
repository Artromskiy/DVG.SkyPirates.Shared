using System;
using System.Runtime.Serialization;

namespace DVG.SkyPirates.Shared.Data
{
    [Serializable]
    [DataContract]
    public partial class PackedCirclesConfig
    {
        [DataMember(Order = 0)]
        public fix Radius;
        [DataMember(Order = 1)]
        public fix2[] Points;

        public PackedCirclesConfig(fix radius, fix2[] points)
        {
            Radius = radius;
            Points = points;
        }
    }
}
