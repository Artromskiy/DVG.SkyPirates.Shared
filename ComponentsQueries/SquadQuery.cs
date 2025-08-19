using Arch.Core;
using DVG.SkyPirates.Shared.Components;

namespace DVG.SkyPirates.Shared.ComponentsQueries
{
    public struct SquadQuery
    {
        private static readonly QueryDescription _query =
            new QueryDescription().WithAll<Squad, Position, Rotation, Direction>();

        public static implicit operator QueryDescription(SquadQuery _) => _query;
    }
}
