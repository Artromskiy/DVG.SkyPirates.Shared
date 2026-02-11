using DVG.Components.Attributes;

namespace DVG.SkyPirates.Shared.Components.Runtime
{
    [Component(true)]
    public struct AutoHeal
    {
        public fix HealPerSecond;
        public fix HealDelay;

        public fix HealLoadPercent;
    }
}
