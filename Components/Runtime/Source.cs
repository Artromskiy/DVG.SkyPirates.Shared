using Delta.ECS;
using DVG.Components.Attributes;

namespace DVG.SkyPirates.Shared.Components.Runtime
{
    [DeltaEcsComponent]
    [Component(true)]
    public struct Source
    {
        public Entity Entity;
    }
}
