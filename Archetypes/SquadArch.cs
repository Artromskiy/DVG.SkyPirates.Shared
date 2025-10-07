using Arch.Core;
using DVG.SkyPirates.Shared.Components;
using DVG.SkyPirates.Shared.Components.Data;

namespace DVG.SkyPirates.Shared.Archetypes
{
    public readonly struct SquadArch
    {
        public static void EnsureArch(World world, Entity entity)
        {
            world.AddOrGet<Squad>(entity);
            world.AddOrGet<Position>(entity);
            world.AddOrGet<Rotation>(entity);
            world.AddOrGet<Direction>(entity);
            world.AddOrGet<Fixation>(entity);
            world.AddOrGet<TargetSearchData>(entity);

            world.AddOrGet<CircleShape>(entity);
            world.AddOrGet<CachePosition>(entity);
            world.AddOrGet<Team>(entity);
        }
    }
}
