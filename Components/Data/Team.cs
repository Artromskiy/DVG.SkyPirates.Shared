using DVG.Core.History.Attributes;

namespace DVG.SkyPirates.Shared.Components.Data
{
    [History]
    public struct Team
    {
        public int Id;

        public override readonly int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
