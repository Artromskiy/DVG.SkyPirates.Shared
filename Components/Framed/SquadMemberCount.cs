using Delta.ECS;
using DVG.Components.Attributes;
using DVG.NewType;

namespace DVG.SkyPirates.Shared.Components.Framed
{
    [DeltaEcsComponent]
    [Component(false)]
    public partial struct SquadMemberCount : INewType
    {
        public int Value;
    }
}
