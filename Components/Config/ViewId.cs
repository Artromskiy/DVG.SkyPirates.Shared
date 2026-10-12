using Delta.ECS;
using DVG.Components.Attributes;

namespace DVG.SkyPirates.Shared.Components.Config
{
    [DeltaEcsComponent]
    [Component(true)]
    public struct ViewToken
    {
        public string Value;
    }
}
