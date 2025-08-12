#region Reals
using real = System.Single;
using real2 = DVG.float2;
using real3 = DVG.float3;
using real4 = DVG.float4;
#endregion

using DVG.Json;
using System;
using System.Runtime.Serialization;

namespace DVG.SkyPirates.Shared.Configs
{
    [JsonAsset]
    [Serializable]
    [DataContract]
    public partial class PackedCirclesConfig
    {
        [DataMember(Order = 0)]
        public real Radius;
        [DataMember(Order = 1)]
        public real2[] Points;
        [DataMember(Order = 2)]
        public int[,] Reorders;

        public PackedCirclesConfig(real radius, real2[] points, int[,] reorders)
        {
            Radius = radius;
            Points = points;
            Reorders = reorders;
        }
    }
}
