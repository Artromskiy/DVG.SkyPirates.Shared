using Arch.Core;
using DVG.SkyPirates.Shared.Components;
using DVG.SkyPirates.Shared.Components.Data;
using DVG.SkyPirates.Shared.Ids;

namespace DVG.SkyPirates.Shared.Archetypes
{
    public readonly struct UnitArch
    {
        public static void EnsureArch(World world, Entity entity)
        {
            world.AddOrGet<UnitId>(entity);
            world.AddOrGet<Health>(entity);
            world.AddOrGet<MaxHealth>(entity);
            world.AddOrGet<Position>(entity);
            world.AddOrGet<Rotation>(entity);
            world.AddOrGet<MoveSpeed>(entity);
            world.AddOrGet<ImpactDistance>(entity);
            world.AddOrGet<Damage>(entity);
            world.AddOrGet<Team>(entity);

            world.AddOrGet<RecivedDamage>(entity);
            world.AddOrGet<AutoHeal>(entity);
            world.AddOrGet<TempPosition>(entity);
            world.AddOrGet<PositionSeparation>(entity);
            world.AddOrGet<Fixation>(entity);
            world.AddOrGet<Target>(entity);
            world.AddOrGet<Destination>(entity);
            world.AddOrGet<TargetSearchData>(entity);
            world.AddOrGet<Behaviour>(entity);
            world.AddOrGet<BehaviourConfig>(entity);
        }
    }
}
