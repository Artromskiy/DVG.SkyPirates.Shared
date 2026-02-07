using Arch.Core;
using DVG.SkyPirates.Shared.Components.Config;
using DVG.SkyPirates.Shared.Components.Framed;
using DVG.SkyPirates.Shared.Components.Runtime;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.IServices
{
    public interface ITargetSearchSystem : ITickableExecutor
    {
        Entity FindTarget(
            ref Position position,
            ref TargetSearchDistance searchDistance,
            ref TargetSearchPosition searchPosition,
            ref Team team);

        void FindTargets(
            ref TargetSearchDistance searchDistance,
            ref TargetSearchPosition searchPosition,
            ref Team team,
            List<Entity> targets);
    }
}