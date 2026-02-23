using Arch.Core;
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
        private static readonly fix _reduceImpactDistance = fix.One / 2;
        private readonly QueryDescription _desc = new QueryDescription().
            WithAll<Destination, Position, ImpactDistance, Target>().NotDisposing().NotDisabled();

        private readonly World _world;

        public SetTargetDestinationSystem(World world)
        {
            _world = world;
        }

        public void Tick(int tick, fix deltaTime)
        {
            var query = new SetTargetDestinationQuery(_world);
            _world.InlineQuery<SetTargetDestinationQuery, Position, Rotation, Destination, ImpactDistance, Target>(_desc, ref query);
        }

        private readonly struct SetTargetDestinationQuery : IForEach<Position, Rotation, Destination, ImpactDistance, Target>
        {
            private readonly World _world;

            public SetTargetDestinationQuery(World world)
            {
                _world = world;
            }

            public void Update(ref Position position, ref Rotation rotation, ref Destination destination, ref ImpactDistance impactDistance, ref Target target)
            {
                if (!target.Entity.HasValue)
                {
                    return;
                }

                destination.Position = position;
                destination.Rotation = rotation;

                fix3 targetPos = _world.Get<Position>(target.Entity.Value);
                var impactReduced = impactDistance - _reduceImpactDistance;
                var impactSqrDistance = impactReduced * impactReduced;

                var sqrDistance = fix3.SqrDistance(targetPos, position);

                if (sqrDistance != 0)
                {
                    var direction = targetPos - position;
                    destination.Rotation = Maths.Degrees(MathsExtensions.GetRotation(direction.xz));
                }

                if (sqrDistance > impactSqrDistance)
                {
                    destination.Position = fix3.MoveTowards(targetPos, position, impactReduced);
                }
            }
        }
    }
}
