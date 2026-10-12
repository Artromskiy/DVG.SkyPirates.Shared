using Delta.ECS;
using DVG.Components.Attributes;

namespace DVG.SkyPirates.Shared.Components.Config
{
    [DeltaEcsComponent]
    [Component(true)]
    public struct Ranged
    {
        public float ProjectileSpeed;
        public string ProjectileViewToken;
    }
}
