using Delta.ECS;
using Delta;
using DVG.Components.Attributes;

namespace DVG.SkyPirates.Shared.Components.Config
{
    [DeltaEcsComponent]
    [Component(true)]
    public struct Separator
    {
        public fix Radius;
        public fix Coefficient;
    }
}
