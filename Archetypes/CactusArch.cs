using Arch.Core;
using DVG.SkyPirates.Shared.Components;
using DVG.SkyPirates.Shared.Components.Config;
using DVG.SkyPirates.Shared.Ids;
using System;

namespace DVG.SkyPirates.Shared.Archetypes
{
    public static class CactusArch
    {
        [Obsolete]
        public static void EnsureArch(World world, Entity entity)
        {
            world.RemoveRange(entity, world.GetSignature(entity).Components);
            world.Add<
                CactusId,
                Position,
                Rotation,
                ImpactDistance,
                TargetSearchData,
                Targets,
                Damage,
                Team,
                Alive,
                Radius,
                BehaviourConfig,
                Separation>
                (entity);
        }
    }
}
