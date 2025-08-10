#region Reals
using real = System.Single;
using real2 = DVG.float2;
using real3 = DVG.float3;
using real4 = DVG.float4;
#endregion

using DVG.Json;
using System;

namespace DVG.SkyPirates.Shared.Models
{
    [JsonAsset]
    [Serializable]
    public partial class UnitModel
    {
        public real health;
        public real damage;
        public real speed;

        public real attackDistance;
        public real damageZone;
        public real bulletSpeed;
        public real reload;
        public real preAttack;
        public real postAttack;
    }
}