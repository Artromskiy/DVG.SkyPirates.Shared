using DVG.Core.Components.Attributes;

namespace DVG.SkyPirates.Shared.Components.Data
{
    [History]
    [Component]
    public struct Position
    {
        public fix3 Value;

        public override readonly int GetHashCode()
        {
            return Value.x.raw + Value.y.raw + Value.z.raw;
        }
    }
}