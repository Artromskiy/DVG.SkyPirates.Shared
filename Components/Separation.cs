using DVG.Core.History.Attributes;

namespace DVG.SkyPirates.Shared.Components
{
    [History]
    public struct Separation
    {
        public fix AddRadius;
        public fix AffectedCoeff;
        public fix AffectingCoeff;

        public fix2 Force;
        public int ForcesCount;

        public override readonly int GetHashCode()
        {
            return AddRadius.raw + AffectedCoeff.raw + AffectingCoeff.raw + Force.x.raw + Force.y.raw;
        }
    }
}
