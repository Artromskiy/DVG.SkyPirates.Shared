using Delta.ECS;
using Delta;
using DVG.Components.Attributes;
using DVG.NewType;

namespace DVG.SkyPirates.Shared.Components.Runtime
{
    [DeltaEcsComponent]
    [Component(true)]
    public partial struct Position : INewType
    {
        public fix3 Value;
    }
}