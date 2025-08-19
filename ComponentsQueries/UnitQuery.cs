using Arch.Core;
using DVG.SkyPirates.Shared.Components;

namespace DVG.SkyPirates.Shared.ComponentsQueries
{
    public readonly struct UnitQuery
    {
        private static readonly QueryDescription _query =
            new QueryDescription().WithAll<Unit, Health, Position, Rotation>();

        public static implicit operator QueryDescription(UnitQuery _) => _query;
    }
}
