using DVG.Core.History.Attributes;
using System;

namespace DVG.SkyPirates.Shared.Components
{
    [History]
    public struct TargetSearchData
    {
        public fix3 Position;
        public fix Distance;

        public override readonly int GetHashCode()
        {
            return HashCode.Combine(Position, Distance);
        }
    }
}
