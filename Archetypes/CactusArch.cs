using Arch.Core;
using DVG.SkyPirates.Shared.Components.Framed;
using DVG.SkyPirates.Shared.Components.Runtime;
using System;

namespace DVG.SkyPirates.Shared.Archetypes
{
    public static class CactusArch
    {
        [Obsolete]
        public static void EnsureArch(World world, Entity entity)
        {
            world.Add<
                Position,
                Rotation,
                TargetSearchPosition,
                Targets,
                Team>
                (entity);
        }
    }
}
