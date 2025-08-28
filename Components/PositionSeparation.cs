using DVG.Core.History.Attributes;

namespace DVG.SkyPirates.Shared.Components
{
    [History]
    public struct PositionSeparation
    {
        public fix Radius;
        public fix Weight;
        public fix2 Force;
    }
}
