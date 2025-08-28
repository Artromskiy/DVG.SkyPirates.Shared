using Arch.Core;
using Arch.Core.Extensions;
using DVG.SkyPirates.Shared.Components;
using DVG.SkyPirates.Shared.Components.Data;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;
using DVG.SkyPirates.Shared.Tools.Extensions;

namespace DVG.SkyPirates.Shared.Services.TickableExecutors.Systems
{
    /// <summary>
    /// If <see href="Target"/> is set, then <see href="Destination"/> will set, so it will be in range of <see href="ImpactDistance"/>
    /// </summary>
    public class SetTargetDestinationSystem : ITickableExecutor
    {
        private readonly QueryDescription _desc = new QueryDescription().WithAll<Destination, Position, Fixation, ImpactDistance, Target>();
        private readonly World _world;

        public SetTargetDestinationSystem(World world)
        {
            _world = world;
        }

        public void Tick(int tick, fix deltaTime)
        {
            _world.InlineQuery<SetTargetDestinationQuery, Destination, Position, ImpactDistance, Target>(_desc);
        }

        private readonly struct SetTargetDestinationQuery : IForEach<Destination, Position, ImpactDistance, Target>
        {
            public void Update(ref Destination destination, ref Position position, ref ImpactDistance impactDistance, ref Target target)
            {
                if (!target.Entity.IsAlive())
                    return;

                var targetPos = target.Entity.Get<Position>().Value;
                var impactSqrDistance = impactDistance.Value * impactDistance.Value;
                var sqrDistance = fix3.SqrDistance(targetPos, position.Value);
                var direction = targetPos - position.Value;
                if(sqrDistance != 0)
                destination.Rotation = Maths.Degrees(MathsExtensions.GetRotation(direction.xz));
                if (sqrDistance < impactSqrDistance)
                {
                    destination.Position = position.Value;
                    return;
                }

                destination.Position = fix3.MoveTowards(targetPos, position.Value, impactDistance.Value);
            }
        }
    }
}
