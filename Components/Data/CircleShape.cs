using DVG.Core.Components.Attributes;

namespace DVG.SkyPirates.Shared.Components.Data
{
    [History]
    [Component]
    public struct CircleShape
    {
        public fix Radius;

        public override readonly int GetHashCode()
        {
            return Radius.raw;
        }
    }
}