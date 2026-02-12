using DVG.Components.Attributes;
using DVG.NewType;

namespace DVG.SkyPirates.Shared.Components.Runtime
{
    [Component(true)]
    public partial struct Health : INewType
    {
        public fix Value;
    }
}
