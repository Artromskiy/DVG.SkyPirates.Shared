using DVG.Components.Attributes;

namespace DVG.SkyPirates.Shared.Components.Config
{
    [Component(true)]
    public struct Separation
    {
        public fix AddRadius;
        public fix AffectingCoeff;
        public fix AffectedCoeff;
    }
}
