using Arch.Core;
using DVG.SkyPirates.Shared.Components;
using DVG.SkyPirates.Shared.Components.Config; using DVG.SkyPirates.Shared.Components.Framed; using DVG.SkyPirates.Shared.Components.Runtime;
using DVG.SkyPirates.Shared.Ids;
using System;

namespace DVG.SkyPirates.Shared.Archetypes
{
    public static class TreeArch
    {
        [Obsolete]
        public static void EnsureArch(World world, Entity entity)
        {
            world.RemoveRange(entity, world.GetSignature(entity).Components);
            world.Add<
                TreeId,
                MaxHealth,
                Position,
                Rotation,
                Team,
                Alive,
                Radius,
                Separation>
                (entity);
        }
    }
}