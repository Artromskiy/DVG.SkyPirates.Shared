using Arch.Core;
using DVG.SkyPirates.Shared.Components;
using DVG.SkyPirates.Shared.Components.Config;
using DVG.SkyPirates.Shared.Components.Framed;
using DVG.SkyPirates.Shared.Components.Runtime;
using System;

namespace DVG.SkyPirates.Shared.Archetypes
{
    public static class SquadArch
    {
        [Obsolete]
        public static void EnsureArch(World world, Entity entity)
        {
            world.RemoveRange(entity, world.GetSignature(entity).Components);
            world.Add<
                Squad,
                Position,
                Rotation,
                Direction,
                Fixation,
                TargetSearchDistance,
                TargetSearchPosition,
                Radius,
                Team>
                (entity);
        }
    }
}
