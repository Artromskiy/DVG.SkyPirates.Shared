using DVG.Core.History.Attributes;
using System;

namespace DVG.SkyPirates.Shared.Components.Data
{
    [History]
    public struct CircleShape
    {
        public fix Radius;

        public override readonly int GetHashCode()
        {
            return HashCode.Combine(Radius);
        }
    }
}
