using Delta.ECS;
using DVG.Components.Attributes;
using DVG.Ids.Attributes;

namespace DVG.SkyPirates.Shared.Ids
{
    [Id(false)]
    [DeltaEcsComponent]
    [Component(true)]
    public partial struct UnitId { }
}