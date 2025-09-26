using DVG.Core.Components.Attributes;

namespace DVG.SkyPirates.Shared.Components.Special
{
    [History]
    [Component]
    internal struct Destruct
    {
        public int TicksPassed;

        public override readonly int GetHashCode() => TicksPassed;
    }
}