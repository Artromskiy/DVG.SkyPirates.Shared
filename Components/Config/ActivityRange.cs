using DVG.Components.Attributes;
using DVG.NewType;

namespace DVG.SkyPirates.Shared.Components.Config
{
    [Component(true)]
    public struct ActivityRange : INewType
    {
        public fix Value;
    }
}
