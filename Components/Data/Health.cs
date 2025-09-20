using DVG.Core.History.Attributes;

namespace DVG.SkyPirates.Shared.Components.Data
{
    [History]
    public struct Health
    {
        public fix Value;

        public override readonly int GetHashCode()
        {
            return Value.raw;
        }
    }
}
