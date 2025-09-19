using DVG.Core.History.Attributes;
using System;

namespace DVG.SkyPirates.Shared.Components
{
    [History]
    public struct Destination
    {
        public fix3 Position;
        public fix Rotation;

        public override readonly int GetHashCode()
        {
            return HashCode.Combine(Position, Rotation);
        }
    }
}
