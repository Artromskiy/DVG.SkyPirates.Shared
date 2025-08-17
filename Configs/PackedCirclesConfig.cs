using System;
using System.Runtime.Serialization;

namespace DVG.SkyPirates.Shared.Configs
{
    [Serializable]
    [DataContract]
    public partial class PackedCirclesConfig
    {
        [DataMember(Order = 0)]
        public fix Radius;
        [DataMember(Order = 1)]
        public fix2[] Points;
        [DataMember(Order = 2)]
        public int[,] Reorders;

        public PackedCirclesConfig(fix radius, fix2[] points, int[,] reorders)
        {
            Radius = radius;
            Points = points;
            Reorders = reorders;
        }
    }
}
