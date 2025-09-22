using DVG.Core.History.Attributes;

namespace DVG.SkyPirates.Shared.Components
{
    [History]
    public struct Alive
    {
        public override readonly int GetHashCode()
        {
            return 100;
        }
    }
}
