using DVG.Core.History.Attributes;
using System;
using System.Runtime.InteropServices;

namespace DVG.SkyPirates.Shared.Components
{
    [History]
    public struct Destination
    {
        public fix3 Position;
        public fix Rotation;

        public override readonly int GetHashCode()
        {
            return Position.x.raw + Position.y.raw + Position.z.raw + Rotation.raw;
        }
    }
}
