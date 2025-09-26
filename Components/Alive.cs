using DVG.Core.Components.Attributes;

namespace DVG.SkyPirates.Shared.Components
{
    [History]
    [Component]
    public struct Alive
    {
        public override readonly int GetHashCode()
        {
            return 100;
        }
    }
}
