using DVG.Core.Components.Attributes;

namespace DVG.SkyPirates.Shared.Components
{
    [History]
    [Component]
    public struct Separation
    {
        public fix AddRadius;
        public fix AffectingCoeff;
        public fix AffectedCoeff;
    }
}
