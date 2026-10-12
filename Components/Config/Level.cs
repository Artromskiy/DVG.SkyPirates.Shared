using Delta.ECS;
using DVG.Components.Attributes;
using DVG.NewType;

namespace DVG.SkyPirates.Shared.Components.Config
{
    [DeltaEcsComponent]
    [Component(true)]
    public partial struct Level : INewType
    {
        public int Value;
    }
}