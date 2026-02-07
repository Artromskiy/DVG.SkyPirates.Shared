using Arch.Core;
using DVG.SkyPirates.Shared.Components;
using DVG.SkyPirates.Shared.Components.Config;
using DVG.SkyPirates.Shared.Components.Framed;
using DVG.SkyPirates.Shared.Components.Runtime;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;
using DVG.SkyPirates.Shared.Tools.Extensions;

namespace DVG.SkyPirates.Shared.Systems
{
    /// <summary>
    /// If <see href="Target"/> is set, then <see href="Destination"/> will set, so it will be in range of <see href="ImpactDistance"/>
    /// </summary>
    public sealed class SetTargetDestinationSystem : ITickableExecutor
    {
        private static readonly fix _reduceImpactDistance = 1;
        private readonly QueryDescription _desc = new QueryDescription().
            WithAll<Destination, Position, Fixation, ImpactDistance, Target>();

        private readonly World _world;

        public SetTargetDestinationSystem(World world)
        {
            _world = world;
        }

        public void Tick(int tick, fix deltaTime)
        {
            var query = new SetTargetDestinationQuery(_world);
            _world.InlineQuery<SetTargetDestinationQuery, Destination, Position, ImpactDistance, Target>(_desc, ref query);
        }

        private readonly struct SetTargetDestinationQuery : IForEach<Destination, Position, ImpactDistance, Target>
        {
            private readonly World _world;

            public SetTargetDestinationQuery(World world)
            {
                _world = world;
            }

            public void Update(ref Destination destination, ref Position position, ref ImpactDistance impactDistance, ref Target target)
            {
                if (!target.Entity.HasValue)
                    return;

                var targetPos = _world.Get<Position>(target.Entity.Value).Value;

                var impactReduced = impactDistance.Value - _reduceImpactDistance;
                var impactSqrDistance = impactReduced * impactReduced;

                var sqrDistance = fix3.SqrDistance(targetPos, position.Value);

                if (sqrDistance != 0)
                {
                    var direction = targetPos - position.Value;
                    destination.Rotation = Maths.Degrees(MathsExtensions.GetRotation(direction.xz));
                }

                if (sqrDistance > impactSqrDistance)
                {
                    destination.Position = fix3.MoveTowards(targetPos, position.Value, impactReduced);
                    return;
                }

                destination.Position = position.Value;
            }
        }
    }
}
