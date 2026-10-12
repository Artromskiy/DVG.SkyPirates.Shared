using Delta.ECS;
using DVG.Components.Attributes;

namespace DVG.SkyPirates.Shared.Components.Framed
{
    [DeltaEcsComponent]
    [Component(false)]
    public struct Target
    {
        public Entity? Entity;
    }
}
