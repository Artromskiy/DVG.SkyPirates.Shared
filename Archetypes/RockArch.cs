using Arch.Core;
using DVG.SkyPirates.Shared.Components;
using DVG.SkyPirates.Shared.Components.Data;
using DVG.SkyPirates.Shared.Ids;

namespace DVG.SkyPirates.Shared.Archetypes
{
    public class RockArch
    {
        public static void EnsureArch(World world, Entity entity)
        {
            world.RemoveRange(entity, world.GetSignature(entity).Components);
            world.Add<
                RockId,
                Health,
                MaxHealth,
                Position,
                Rotation,
                Team,
                Alive,
                CircleShape,
                RecivedDamage,
                AutoHeal,
                Separation>
                (entity);
        }
    }
}