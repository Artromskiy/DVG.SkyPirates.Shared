using Arch.Core;
using DVG.SkyPirates.Shared.Components;

namespace DVG.SkyPirates.Shared.Archetypes
{
    public readonly struct TargetArch
    {
        private static readonly QueryDescription _query = new QueryDescription().WithAll<
            Health,
            Position,
            Team>();

        public static implicit operator QueryDescription(TargetArch _) => _query;
    }
}
