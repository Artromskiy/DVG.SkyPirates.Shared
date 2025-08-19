using DVG.SkyPirates.Shared.Configs;

namespace DVG.SkyPirates.Shared.Components
{
    public struct Unit
    {
        public fix3 TargetPosition;
        public fix TargetRotation;
        public fix PreAttack;
        public fix PostAttack;

        public UnitConfig UnitConfig;
    }
}
