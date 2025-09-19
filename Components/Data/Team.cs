using DVG.Core.History.Attributes;
using System;

namespace DVG.SkyPirates.Shared.Components.Data
{
    [History]
    public struct Team
    {
        public int Id;

        public override readonly int GetHashCode()
        {
            return HashCode.Combine(Id);
        }
    }
}
