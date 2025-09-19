using DVG.Core.History.Attributes;
using System.Runtime.InteropServices;

namespace DVG.SkyPirates.Shared.Components.Data
{
    [History]
    public struct CircleShape
    {
        public fix Radius;

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
