using Arch.Core;
using DVG.SkyPirates.Shared.Components;
using DVG.SkyPirates.Shared.Components.Config;
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
            world.RemoveRange(entity, world.GetSignature(entity).Components);
            world.Add<

                // Synced, loaded first by config
                // Config Data

                // Synced, edited by systems
                // Runtime Data
                Position,
                Rotation,
                Team,
                Alive,

                // Non Synced, edited by systems
                // System Frame Data
                Fixation,
                Target,
                Destination,
                TargetSearchDistance,
                TargetSearchPosition>

                // Special Data (History, Dispose, Free, Temp)
                (entity);
        }
    }
}