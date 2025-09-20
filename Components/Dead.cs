using DVG.Core.History.Attributes;

namespace DVG.SkyPirates.Shared.Components
{
    [History]
    public struct Dead
    {
        public override readonly int GetHashCode()
        {
            return 100;
        }
    }
}
