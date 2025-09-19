using DVG.Core.History.Attributes;
using System;
using System.Runtime.InteropServices;

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
            var tt = this;
            var span = MemoryMarshal.CreateSpan(ref tt, 1);
            int hash = 0;
            foreach (var item in MemoryMarshal.AsBytes(span))
                hash += item;

            return hash;
        }
    }
}
