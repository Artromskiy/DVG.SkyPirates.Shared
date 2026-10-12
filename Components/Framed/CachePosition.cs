using Delta.ECS;
using Delta;
using DVG.Components.Attributes;
using DVG.NewType;

namespace DVG.SkyPirates.Shared.Components.Framed
{
    [DeltaEcsComponent]
    [Component(false)]
    public partial struct CachePosition : INewType
    {
        public fix3 Value;
    }
}
