using DVG.Core.History.Attributes;
using System;

namespace DVG.SkyPirates.Shared.Components
{
    [History]
    public struct AutoHeal
    {
        public fix healPerSecond;
        public fix healDelay;

        public fix healLoadPercent;

        public override readonly int GetHashCode()
        {
            return HashCode.Combine(healPerSecond, healDelay, healLoadPercent);
        }
    }
}
