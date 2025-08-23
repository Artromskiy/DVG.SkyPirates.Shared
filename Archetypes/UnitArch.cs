using Arch.Core;
using Arch.Core.Extensions;
using DVG.SkyPirates.Shared.Components;
using DVG.SkyPirates.Shared.Components.Data;

namespace DVG.SkyPirates.Shared.Archetypes
{
    public readonly struct UnitArch
    {
        public static void EnsureArch(Entity entity)
        {
            entity.AddOrGet<Health>();
            entity.AddOrGet<Position>();
            entity.AddOrGet<Rotation>();
            entity.AddOrGet<MoveSpeed>();
            entity.AddOrGet<ImpactDistance>();
            entity.AddOrGet<Damage>();
            entity.AddOrGet<Team>();

            entity.AddOrGet<Fixation>();
            entity.AddOrGet<Target>();
            entity.AddOrGet<Destination>();
            entity.AddOrGet<Behaviour>();
            entity.AddOrGet<BehaviourConfig>();
        }
    }
}
