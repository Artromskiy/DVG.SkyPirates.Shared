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

        public override readonly int GetHashCode()
        {
            return healPerSecond.raw + healDelay.raw + healLoadPercent.raw;
        }
    }
}
