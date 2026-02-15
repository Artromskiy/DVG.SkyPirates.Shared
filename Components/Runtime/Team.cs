using DVG.Components.Attributes;
using System;

namespace DVG.SkyPirates.Shared.Components.Runtime
{
    [Component(true)]
    [Obsolete("Rename to TeamId and use NewType")]
    public struct Team
    {
        public int Id;
    }
}