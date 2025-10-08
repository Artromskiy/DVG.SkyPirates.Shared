using DVG.Core.Components.Attributes;

namespace DVG.SkyPirates.Shared.Components.Data
{
    [History]
    [Component]
    public partial struct Level
    {
        public int Value;
        public override readonly int GetHashCode() => Value;
    }
}