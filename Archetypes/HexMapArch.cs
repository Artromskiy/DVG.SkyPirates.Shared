using Arch.Core;

namespace DVG.SkyPirates.Shared.Archetypes
{
    public class HexMapArch
    {
        public static void EnsureArch(World world, Entity entity)
        {
            world.RemoveRange(entity, world.GetSignature(entity).Components);
            world.Add<HexMapArch>(entity);
        }
    }
}
