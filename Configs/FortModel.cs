#region Reals
using real = System.Single;
using real2 = DVG.float2;
using real3 = DVG.float3;
using real4 = DVG.float4;
#endregion

using DVG.Json;
using System;

namespace DVG.SkyPirates.Shared.Configs
{
    [JsonAsset]
    [Serializable]
    public partial class FortModel
    {
        public int health;
        public int cannons;
        public int damage;
        public real reloadTime;
        public real attackRadius;
        public real projectileSpeed;
        public int maxHold;
        public int level;
        public int maxLevel;
    }
}
