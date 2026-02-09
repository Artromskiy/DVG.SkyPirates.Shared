using Arch.Core;
using DVG.SkyPirates.Shared.Components.Config;
using DVG.SkyPirates.Shared.Components.Runtime;
using System;

namespace DVG.SkyPirates.Shared.Archetypes
{
    public static class SquadArch
    {
        [Obsolete]
        public static void EnsureArch(World world, Entity entity)
        {
            world.Add<
                Squad,
                Position,
                Rotation,
                Direction,
                Fixation,
                MaxSpeed,
                TargetSearchDistance,
                Radius,
                Team>
                (entity);
        }
    }
}
