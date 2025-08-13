using DVG.Json;
using System;

namespace DVG.SkyPirates.Shared.Configs
{
    [JsonAsset]
    [Serializable]
    public partial class CameraConfig
    {
        public fix minXAngle;
        public fix maxXAngle;
        public fix minFov;
        public fix maxFov;
        public fix minDistance;
        public fix maxDistance;
        public fix smoothMoveTime;
    }
}