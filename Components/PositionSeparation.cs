using DVG.Core.History.Attributes;

namespace DVG.SkyPirates.Shared.Components
{
    [History]
    public struct PositionSeparation
    {
        public fix Radius;
        public fix Weight;
        public fix2 Force;

        public override readonly int GetHashCode()
        {
            return Radius.raw + Weight.raw + Force.x.raw + Force.y.raw;
        }
    }
}
