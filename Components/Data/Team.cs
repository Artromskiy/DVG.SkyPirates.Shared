using DVG.Core.Components.Attributes;

namespace DVG.SkyPirates.Shared.Components.Data
{
    [History]
    [Component]
    public struct Team
    {
        public int Id;

        public override readonly int GetHashCode()
        {
            return Id;
        }
    }
}
