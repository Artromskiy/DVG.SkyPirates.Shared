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
            world.RemoveRange(entity, world.GetSignature(entity).Components);
            world.Add<
                UnitId,
                Health,
                MaxHealth,
                Position,
                Rotation,
                MoveSpeed,
                ImpactDistance,
                Damage,
                Team,
                Alive,
                CircleShape,
                CachePosition,
                RecivedDamage,
                AutoHeal,
                Separation,
                Fixation,
                Target,
                Destination,
                TargetSearchData,
                Behaviour,
                BehaviourConfig>
                (entity);
        }
    }
}