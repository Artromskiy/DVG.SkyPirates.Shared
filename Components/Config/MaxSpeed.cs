using Delta.ECS;
using Delta;
using DVG.Components.Attributes;
using DVG.NewType;

namespace DVG.SkyPirates.Shared.Components.Config
{
    [DeltaEcsComponent]
    [Component(true)]
    public partial struct MaxSpeed : INewType
    {
        public fix Value;
    }
}