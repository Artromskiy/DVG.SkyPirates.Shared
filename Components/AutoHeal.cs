using DVG.Core.Components.Attributes;

namespace DVG.SkyPirates.Shared.Components
{
    [History]
    [Component]
    public struct AutoHeal
    {
        public fix healPerSecond;
        public fix healDelay;

        public fix healLoadPercent;
    }
}
