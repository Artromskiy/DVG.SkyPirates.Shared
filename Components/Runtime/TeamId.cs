using DVG.Components.Attributes;
using DVG.NewType;

namespace DVG.SkyPirates.Shared.Components.Runtime
{
    [Component(true)]
    public struct TeamId : INewType
    {
        public int Value;
    }
}