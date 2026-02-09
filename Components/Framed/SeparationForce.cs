using DVG.Core.Components.Attributes;
using System;

namespace DVG.SkyPirates.Shared.Components.Framed
{
    [Component(false)]
    [Obsolete]
    public struct SeparationForce
    {
        public fix2 Force;
        public int ForcesCount;
    }
}
