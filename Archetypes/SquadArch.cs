using Arch.Core;
using Arch.Core.Extensions;
using DVG.SkyPirates.Shared.Components;

namespace DVG.SkyPirates.Shared.Archetypes
{
    public readonly struct SquadArch
    {
        private static readonly QueryDescription _query = new QueryDescription().WithAll<
            Squad,
            Position,
            Rotation,
            Direction,
            Fixation>();

        public static QueryDescription GetQuery() => _query;

        public static void EnsureArch(Entity entity)
        {
            entity.AddOrGet<Squad>();
            entity.AddOrGet<Position>();
            entity.AddOrGet<Rotation>();
            entity.AddOrGet<Direction>();
            entity.AddOrGet<Fixation>();
        }
    }
}
