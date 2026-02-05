using DVG.Core.Components.Attributes;

namespace DVG.SkyPirates.Shared.Components.Config
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
