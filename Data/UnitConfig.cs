using System;
using System.Runtime.Serialization;

namespace DVG.SkyPirates.Shared.Data
{
    [Serializable]
    [DataContract]
    public partial class UnitConfig
    {
        [DataMember(Order = 0)]
        public fix health;
        [DataMember(Order = 1)]
        public fix damage;
        [DataMember(Order = 2)]
        public fix speed;

        [DataMember(Order = 3)]
        public fix attackDistance;
        [DataMember(Order = 4)]
        public fix damageZone;
        [DataMember(Order = 5)]
        public fix bulletSpeed;
        [DataMember(Order = 6)]
        public fix reload;
        [DataMember(Order = 7)]
        public fix preAttack;
        [DataMember(Order = 8)]
        public fix postAttack;
    }
}