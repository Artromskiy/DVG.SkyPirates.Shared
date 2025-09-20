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
            return Radius.raw;
        }
    }
}
