using System;

namespace DVG.SkyPirates.Shared.Configs
{
    [Serializable]
    public partial class UnitConfig
    {
        public fix health;
        public fix damage;
        public fix speed;

        public fix attackDistance;
        public fix damageZone;
        public fix bulletSpeed;
        public fix reload;
        public fix preAttack;
        public fix postAttack;
    }
}