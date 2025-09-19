using DVG.Core.History.Attributes;
using System;

namespace DVG.SkyPirates.Shared.Components.Data
{
    [History]
    public struct Health
    {
        public fix Value;

        public override readonly int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }
}
