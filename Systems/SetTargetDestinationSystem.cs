using Delta;
using Delta.ECS;
using DVG.SkyPirates.Shared.Ecs;
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
    public sealed class SetTargetDestinationSystem : IDeltaTickableExecutor
    {
        private static readonly fix _reduceImpactDistance = fix.One / 2;
        private Query? _descCache;
        private Query _desc => _descCache ??= _world.
            WhereAll<Destination, Position, Rotation, ImpactDistance, Target>().Alive().NotDisabled();

        private readonly World _world;

        public SetTargetDestinationSystem(World world)
        {
            _world = world;
        }

        public void Tick(int tick, fix deltaTime)
        {
            var world = _world;
            var desc = _desc;
            _world.ForEach<World, Position, Rotation, Destination, ImpactDistance, Target>(in desc, ref world,
                static (ref World world, ref Position position, ref Rotation rotation, ref Destination destination, ref ImpactDistance impactDistance, ref Target target) =>
                {
                if (!target.Entity.HasValue)
                {
                    return;
                }

                destination.Position = position;
                destination.Rotation = rotation;

                fix3 targetPos = world.Get<Position>(target.Entity.Value);
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
                });
        }
    }
}
