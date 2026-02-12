using DVG.Components.Attributes;
using DVG.NewType;

namespace DVG.SkyPirates.Shared.Components.Config
{
    [Component(true)]
    public partial struct Level : INewType
    {
        public int Value;
    }
}