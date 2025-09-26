using Arch.Core;
using DVG.Core.Components.Attributes;

namespace DVG.SkyPirates.Shared.Components
{
    [History]
    [Component]
    public struct Target
    {
        public Entity? Entity;

        public override readonly int GetHashCode()
        {
            return Entity.HasValue ? Entity.Value.Id : 0;
        }
    }
}
