using DVG.Core.History.Attributes;

namespace DVG.SkyPirates.Shared.Components
{
    [History]
    public struct Direction
    {
        public fix2 Value;

        public override readonly int GetHashCode()
        {
            return Value.x.raw + Value.y.raw;
        }
    }
}
