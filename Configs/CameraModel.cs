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
    public partial class CameraModel
    {
        public real minXAngle;
        public real maxXAngle;
        public real minFov;
        public real maxFov;
        public real minDistance;
        public real maxDistance;
        public real smoothMoveTime;
    }
}