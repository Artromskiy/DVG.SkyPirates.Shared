using Arch.Core;
using DVG.SkyPirates.Shared.Components.Data;

namespace DVG.SkyPirates.Shared.Archetypes
{
    public static class HexMapArch
    {
        public static void EnsureArch(World world, Entity entity)
        {
            world.RemoveRange(entity, world.GetSignature(entity).Components);
            world.Add<HexMap>(entity);
        }
    }
}
