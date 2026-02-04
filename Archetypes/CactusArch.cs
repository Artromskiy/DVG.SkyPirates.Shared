using Arch.Core;
using DVG.SkyPirates.Shared.Components;
using DVG.SkyPirates.Shared.Components.Data;
using DVG.SkyPirates.Shared.Ids;

namespace DVG.SkyPirates.Shared.Archetypes
{
    public static class CactusArch
    {
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
                CircleShape,
                BehaviourState,
                BehaviourConfig,
                Separation>
                (entity);
        }
    }
}
