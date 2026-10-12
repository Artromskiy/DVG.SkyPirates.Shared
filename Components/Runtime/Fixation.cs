using Delta.ECS;
using DVG.Components.Attributes;
using DVG.NewType;

namespace DVG.SkyPirates.Shared.Components.Runtime
{
    [DeltaEcsComponent]
    [Component(true)]
    public partial struct Fixation : INewType
    {
        public bool Value;
    }
}
