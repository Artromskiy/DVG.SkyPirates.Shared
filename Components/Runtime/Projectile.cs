using Delta.ECS;
using Delta;
using DVG.Components.Attributes;

namespace DVG.SkyPirates.Shared.Components.Runtime
{
    [DeltaEcsComponent]
    [Component(true)]
    public struct Projectile
    {
        public Entity Target;
        public fix3 StartPosition;
    }
}
