using DVG.Core.History.Attributes;
using System;

namespace DVG.SkyPirates.Shared.Components.Data
{
    [History]
    public struct ImpactDistance
    {
        public fix Value;

        public override readonly int GetHashCode()
        {
            return HashCode.Combine(Value);
        }
    }
}