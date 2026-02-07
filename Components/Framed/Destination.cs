using DVG.Core.Components.Attributes;

namespace DVG.SkyPirates.Shared.Components.Framed
{
    [History]
    [Component]
    public struct Destination
    {
        public fix3 Position;
        public fix Rotation;
    }
}
