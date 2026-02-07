using Arch.Core;
using DVG.SkyPirates.Shared.Components.Runtime;
using System;

namespace DVG.SkyPirates.Shared.Archetypes
{
    public static class RockArch
    {
        [Obsolete]
        public static void EnsureArch(World world, Entity entity)
        {
            world.RemoveRange(entity, world.GetSignature(entity).Components);
            world.Add<
                Position,
                Rotation,
                Team>
                (entity);
        }
    }
}