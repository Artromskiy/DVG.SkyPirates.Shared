using Delta.ECS;
using DVG.Components.Attributes;
using DVG.NewType;

namespace DVG.SkyPirates.Shared.Components.Config
{
    [DeltaEcsComponent]
    [Component(true)]
    public partial struct GoodsAmount : INewType
    {
        public int Value;
    }
}
