using Arch.Core;
using Arch.Core.Extensions;
using DVG.SkyPirates.Shared.Components;
using DVG.SkyPirates.Shared.Components.Data;

namespace DVG.SkyPirates.Shared.Archetypes
{
    public readonly struct UnitArch
    {
        private static readonly QueryDescription _query = new QueryDescription().WithAll<
            Health,
            Position,
            Rotation,
            Fixation,
            Behaviour,
            Destination,
            ImpactDistance,
            Target,
            Damage,
            Team>();

        public static QueryDescription GetQuery() => _query;

        public static void EnsureArch(Entity entity)
        {
            entity.AddOrGet<Health>();
            entity.AddOrGet<Position>();
            entity.AddOrGet<Rotation>();
            entity.AddOrGet<Fixation>();
            entity.AddOrGet<Behaviour>();
            entity.AddOrGet<Destination>();
            entity.AddOrGet<ImpactDistance>();
            entity.AddOrGet<Target>();
            entity.AddOrGet<Damage>();
            entity.AddOrGet<Team>();
        }
    }
}
