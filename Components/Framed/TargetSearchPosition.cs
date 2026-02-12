using DVG.Components.Attributes;
using DVG.NewType;

namespace DVG.SkyPirates.Shared.Components.Framed
{
    [Component(false)]
    public partial struct TargetSearchPosition : INewType
    {
        public fix3 Value;
    }
}
