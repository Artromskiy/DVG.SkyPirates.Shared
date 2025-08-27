using Arch.Core;
using Arch.Core.Extensions;
using DVG.SkyPirates.Shared.Components;
using DVG.SkyPirates.Shared.Components.Data;

namespace DVG.SkyPirates.Shared.Archetypes
{
    public readonly struct SquadArch
    {
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
