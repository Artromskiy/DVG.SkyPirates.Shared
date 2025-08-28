using DVG.Core.History.Attributes;

namespace DVG.SkyPirates.Shared.Components
{
    [History]
    public struct AutoHeal
    {
        public fix healPerSecond;
        public fix healDelay;

        public fix healLoadPercent;
    }
}
