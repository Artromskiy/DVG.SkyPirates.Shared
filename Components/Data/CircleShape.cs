using DVG.Core.History.Attributes;

namespace DVG.SkyPirates.Shared.Components.Data
{
    [History]
    public struct CircleShape
    {
        public fix Radius;

        public override readonly int GetHashCode()
        {
            return Radius.GetHashCode();
        }
    }
}
