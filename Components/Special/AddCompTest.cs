using DVG.Core.History.Attributes;

namespace DVG.SkyPirates.Shared.Components.Special
{
    [History]
    public struct AddCompTest
    {
        public override readonly int GetHashCode()
        {
            return 100;
        }
    }
}
