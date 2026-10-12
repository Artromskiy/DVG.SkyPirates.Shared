using Delta.ECS;
using DVG.Components.Attributes;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Components.Framed
{
    [DeltaEcsComponent]
    [Component(false)]
    public struct Targets
    {
        public List<Entity> Entities;
    }
}
