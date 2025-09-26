using DVG.Core.Components.Attributes;

namespace DVG.SkyPirates.Shared.Components
{
    [History]
    [Component]
    public struct Fixation
    {
        public bool Value;

        public override readonly int GetHashCode()
        {
            return Value ? 1 : 0;
        }
    }
}
