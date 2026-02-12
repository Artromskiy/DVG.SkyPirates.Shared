using DVG.Components.Attributes;
using DVG.NewType;

namespace DVG.SkyPirates.Shared.Components.Config
{
    [Component(true)]
    public partial struct TargetSearchDistance : INewType
    {
        public fix Value;
    }
}
