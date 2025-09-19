using Arch.Core;
using DVG.Core.History.Attributes;

namespace DVG.SkyPirates.Shared.Components
{
    [History]
    public struct Target
    {
        public Entity? Entity;

        public override readonly int GetHashCode()
        {
            return Entity.HasValue ? Entity.Value.Id.GetHashCode() : 0;
        }
    }
}
