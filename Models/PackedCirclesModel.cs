using DVG.Json;
using System;
using System.Runtime.Serialization;

namespace DVG.SkyPirates.Shared.Models
{
    [JsonAsset]
    [Serializable]
    [DataContract]
    public partial class PackedCirclesModel
    {
        [DataMember(Order = 0)]
        public float Radius;
        [DataMember(Order = 1)]
        public float2[] Points;
        [DataMember(Order = 2)]
        public int[,] Reorders;

        public PackedCirclesModel(float radius, float2[] points, int[,] reorders)
        {
            Radius = radius;
            Points = points;
            Reorders = reorders;
        }
    }
}
