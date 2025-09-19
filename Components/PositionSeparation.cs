using DVG.Core.History.Attributes;
using System.Runtime.InteropServices;

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
            var tt = this;
            var span = MemoryMarshal.CreateSpan(ref tt, 1);
            int hash = 0;
            foreach (var item in MemoryMarshal.AsBytes(span))
                hash += item;

            return hash;
        }
    }
}
