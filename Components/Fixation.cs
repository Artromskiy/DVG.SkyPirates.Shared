using DVG.Core.History.Attributes;

namespace DVG.SkyPirates.Shared.Components
{
    [History]
    public struct Fixation
    {
        public bool Value;

        public override readonly int GetHashCode()
        {
            return Value ? 1 : 0;
        }
    }
}
