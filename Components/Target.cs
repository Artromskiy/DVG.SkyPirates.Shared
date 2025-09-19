using Arch.Core;
using DVG.Core.History.Attributes;
using System;

namespace DVG.SkyPirates.Shared.Components
{
    [History]
    public struct Target
    {
        public Entity? Entity;

        public override readonly int GetHashCode()
        {
            return Entity.HasValue ? HashCode.Combine(Entity.Value.Id) : 0;
        }
    }
}
