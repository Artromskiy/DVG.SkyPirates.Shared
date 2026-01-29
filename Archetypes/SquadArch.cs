using Arch.Core;
using DVG.SkyPirates.Shared.Components;
using DVG.SkyPirates.Shared.Components.Data;

namespace DVG.SkyPirates.Shared.Archetypes
{
    public static class SquadArch
    {
        public static void EnsureArch(World world, Entity entity)
        {
            world.RemoveRange(entity, world.GetSignature(entity).Components);
            world.Add<
                Squad,
                Position,
                Rotation,
                Direction,
                Fixation,
                TargetSearchData,
                CircleShape,
                CachePosition,
                Team>
                (entity);
        }
    }
}
