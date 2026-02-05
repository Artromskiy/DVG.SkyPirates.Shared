using DVG.Core.Components.Attributes;

namespace DVG.SkyPirates.Shared.Components.Runtime
{
    [History]
    [Component]
    public struct AutoHeal
    {
        public fix HealPerSecond;
        public fix HealDelay;

        public fix HealLoadPercent;
    }
}
