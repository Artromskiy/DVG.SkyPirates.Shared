using DVG.Core.Components.Attributes;

namespace DVG.SkyPirates.Shared.Components.Data
{
    [History]
    [Component]
    public struct ImpactDistance
    {
        public fix Value;

        public override readonly int GetHashCode()
        {
            return Value.raw;
        }
    }
}