using DVG.Core.Components.Attributes;

namespace DVG.SkyPirates.Shared.Components
{
    [History]
    [Component]
    public struct Destination
    {
        public fix3 Position;
        public fix Rotation;

        public override readonly int GetHashCode()
        {
            return Position.x.raw + Position.y.raw + Position.z.raw + Rotation.raw;
        }
    }
}
