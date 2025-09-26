using DVG.Core.History.Attributes;

namespace DVG.SkyPirates.Shared.Components.Special
{
    [History]
    internal struct Destruct
    {
        public int TicksPassed;

        public override readonly int GetHashCode() => TicksPassed;
    }
}