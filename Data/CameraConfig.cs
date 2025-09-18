using System;
using System.Runtime.Serialization;

namespace DVG.SkyPirates.Shared.Data
{
    [Serializable]
    [DataContract]
    public partial class CameraConfig
    {
        [DataMember(Order = 0)]
        public fix minXAngle;
        [DataMember(Order = 1)]
        public fix maxXAngle;
        [DataMember(Order = 2)]
        public fix minFov;
        [DataMember(Order = 3)]
        public fix maxFov;
        [DataMember(Order = 4)]
        public fix minDistance;
        [DataMember(Order = 5)]
        public fix maxDistance;
        [DataMember(Order = 6)]
        public fix smoothMoveTime;
    }
}