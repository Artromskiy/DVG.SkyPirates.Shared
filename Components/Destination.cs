using DVG.Core.History.Attributes;

namespace DVG.SkyPirates.Shared.Components
{
    [History]
    public struct Destination
    {
        public fix3 Position;
        public fix Rotation;
    }
}
