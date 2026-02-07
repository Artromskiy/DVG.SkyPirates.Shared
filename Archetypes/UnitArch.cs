using Arch.Core;
using DVG.SkyPirates.Shared.Components.Framed;
using DVG.SkyPirates.Shared.Components.Runtime;
using System;

namespace DVG.SkyPirates.Shared.Archetypes
{
    [Obsolete]
    public static class UnitArch
    {
        public static void EnsureArch(World world, Entity entity)
        {
            world.Add<
                Position,
                Rotation,
                Team,
                Target,
                Destination,
                TargetSearchPosition>
                (entity);
        }
    }
}