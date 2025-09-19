using DVG.Core.History.Attributes;
using System;

namespace DVG.SkyPirates.Shared.Components
{
    [History]
    public struct Direction
    {
        public fix2 Value;

        public override readonly int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }
}
