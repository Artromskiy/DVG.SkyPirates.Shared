using DVG.Core.Components.Attributes;

namespace DVG.SkyPirates.Shared.Components
{
    [History]
    [Component]
    public struct TargetSearchData
    {
        public fix3 Position;
        public fix Distance;
    }
}
