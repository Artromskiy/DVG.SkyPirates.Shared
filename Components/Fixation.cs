using DVG.Core.History.Attributes;
using System;

namespace DVG.SkyPirates.Shared.Components
{
    [History]
    public struct Fixation
    {
        public bool Value;

        public override readonly int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }
}
