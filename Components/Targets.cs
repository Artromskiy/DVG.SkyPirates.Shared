using Arch.Core;
using DVG.Core.Components.Attributes;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Components
{
    [History]
    [Component]
    public struct Targets
    {
        public List<Entity> Entities;
    }
}
