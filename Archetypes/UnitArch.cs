using Arch.Core;
using Arch.Core.Extensions;
using DVG.SkyPirates.Shared.Components;

namespace DVG.SkyPirates.Shared.Archetypes
{
    public readonly struct UnitArch
    {
        private static readonly QueryDescription _query = new QueryDescription().WithAll<
            Unit,
            Health,
            Position,
            Rotation,
            Fixation,
            Team>();

        public static QueryDescription GetQuery() => _query;

        public static void EnsureArch(Entity entity)
        {
            entity.AddOrGet<Unit>();
            entity.AddOrGet<Health>();
            entity.AddOrGet<Position>();
            entity.AddOrGet<Rotation>();
            entity.AddOrGet<Fixation>();
            entity.AddOrGet<Team>();
        }
    }
}
