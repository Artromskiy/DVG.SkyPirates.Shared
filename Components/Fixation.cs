using DVG.Core.History.Attributes;
using System;
using System.Runtime.InteropServices;

namespace DVG.SkyPirates.Shared.Components
{
    [History]
    public struct Fixation
    {
        public bool Value;

        public override readonly int GetHashCode()
        {
            return Value ? 1 : 0;
        }
    }
}
