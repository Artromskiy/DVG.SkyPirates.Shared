using System;

namespace DVG.SkyPirates.Shared.Configs
{
    [Serializable]
    public partial class FortModel
    {
        public int health;
        public int cannons;
        public int damage;
        public fix reloadTime;
        public fix attackRadius;
        public fix projectileSpeed;
        public int maxHold;
        public int level;
        public int maxLevel;
    }
}
