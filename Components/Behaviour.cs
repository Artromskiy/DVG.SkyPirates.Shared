using DVG.Core.History.Attributes;
using DVG.SkyPirates.Shared.Ids;
using System;
using System.Runtime.InteropServices;

namespace DVG.SkyPirates.Shared.Components
{
    [History]
    public struct Behaviour
    {
        public StateId State;
        public fix Percent;
        public fix Duration;

        public StateId? ForceState;

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
