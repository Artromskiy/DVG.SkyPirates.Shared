using Delta.ECS;
using Delta;
using DVG.Components.Attributes;

namespace DVG.SkyPirates.Shared.Components.Runtime
{
    [DeltaEcsComponent]
    [Component(true)]
    public struct FlyDestination
    {
        public fix3 StartPosition;
        public fix3 EndPosition;
    }
}
