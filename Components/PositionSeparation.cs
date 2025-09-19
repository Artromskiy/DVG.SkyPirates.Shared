using DVG.Core.History.Attributes;
using System;

namespace DVG.SkyPirates.Shared.Components
{
    [History]
    public struct PositionSeparation
    {
        public fix Radius;
        public fix Weight;
        public fix2 Force;

        public override readonly int GetHashCode()
        {
            return HashCode.Combine(Radius, Weight, Force);
        }
    }
}
