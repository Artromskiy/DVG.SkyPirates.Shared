using DVG.Core.History.Attributes;

namespace DVG.SkyPirates.Shared.Components
{
    [History]
    public struct TargetSearchData
    {
        public fix3 Position;
        public fix Distance;

        public override readonly int GetHashCode()
        {
            return Position.x.raw + Position.y.raw + Position.z.raw + Distance.raw;
        }
    }
}
