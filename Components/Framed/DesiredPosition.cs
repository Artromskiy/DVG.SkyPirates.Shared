using Delta.ECS;
using Delta;
using DVG.Components.Attributes;

namespace DVG.SkyPirates.Shared.Components.Framed
{
    [DeltaEcsComponent]
    [Component(true)]
    public struct DesiredPosition
    {
        public fix3 Value;
    }
}
