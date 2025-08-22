using Arch.Core;
using Arch.Core.Extensions;
using DVG.SkyPirates.Shared.Components;
using DVG.SkyPirates.Shared.Components.Data;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;
using DVG.SkyPirates.Shared.Tools.Extensions;

namespace DVG.SkyPirates.Shared.Services.TickableExecutors.BehaviourSystems
{
    /// <summary>
    /// If <see href="Target"/> is set, then <see href="Destination"/> will set, so it will be in range of <see href="ImpactDistance"/>
    /// </summary>
    public class SetTargetDestinationSystem : ITickableExecutor
    {
        private readonly World _world;

        public SetTargetDestinationSystem(World world)
        {
            _world = world;
        }

        public void Tick(int tick, fix deltaTime)
        {
            var desc = new QueryDescription().WithAll<Destination, Position, Fixation, ImpactDistance, Target>();
            _world.InlineQuery<SetTargetDestinationQuery, Destination, Position, ImpactDistance, Target>(desc);
        }

        private readonly struct SetTargetDestinationQuery : IForEach<Destination, Position, ImpactDistance, Target>
        {
            public void Update(ref Destination destination, ref Position position, ref ImpactDistance impactDistance, ref Target target)
            {
                if (!target.Entity.IsAlive())
                    return;

                var targetPos = target.Entity.Get<Position>().Value;
                var impactSqrDistance = impactDistance.Value * impactDistance.Value;
                var direction = targetPos - position.Value;
                destination.Rotation = Maths.Degrees(MathsExtensions.GetRotation(direction.xz));
                if (fix3.SqrDistance(targetPos, position.Value) < impactSqrDistance)
                    return;

                destination.Position = fix3.MoveTowards(targetPos, position.Value, impactDistance.Value);
            }
        }
    }
}
