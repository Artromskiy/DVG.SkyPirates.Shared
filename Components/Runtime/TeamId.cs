using DVG.Components.Attributes;
using DVG.NewType;

namespace DVG.SkyPirates.Shared.Components.Runtime
{
    [Component(true)]
    public partial struct TeamId : INewType
    {
        public int Value;
    }
}