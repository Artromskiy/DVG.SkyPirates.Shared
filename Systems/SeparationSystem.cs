using Arch.Core;
using DVG.SkyPirates.Shared.Components.Config;
using DVG.SkyPirates.Shared.Components.Framed;
using DVG.SkyPirates.Shared.Components.Runtime;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;
using DVG.SkyPirates.Shared.Systems.Special;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Systems
{
    /// <summary>
    /// Moves Entity's <see href="Position"/> and <see href="Rotation"/> 
    /// with speed <see href="MoveSpeed"/> towards <see href="Destination"/>
    /// </summary>
    public sealed class SeparationSystem : ITickableExecutor
    {
        private const int SquareSize = 1;

        private readonly QueryDescription _affectedDesc = new QueryDescription().
            WithAll<Position, Separation>().NotDisposing();

        private readonly QueryDescription _affectingDesc = new QueryDescription().
            WithAll<Position, Separation, Radius>().NotDisposing();

        //private readonly Lookup2D<List<Entity>> _partitioning = new();
        private readonly Dictionary<int2, List<Entity>> _partitioning = new();
        private readonly List<Entity> _targetsCache = new();

        private readonly World _world;
        public SeparationSystem(World world)
        {
            _world = world;
        }

        public void Tick(int tick, fix deltaTime)
        {
            foreach (var quadrant in _partitioning)
                quadrant.Value.Clear();


            var partitionQuery = new PartitioningQuery(_partitioning);
            _world.InlineEntityQuery<PartitioningQuery, Position>(_affectedDesc, ref partitionQuery);

            var forceQuery = new SeparationForceQuery(_world, _partitioning, _targetsCache);
            _world.InlineQuery<SeparationForceQuery, Position, Separation, Radius>(_affectingDesc, ref forceQuery);

            var separationQuery = new SeparationQuery();
            _world.InlineQuery<SeparationQuery, Position, Separation, SeparationForce>(_affectedDesc, ref separationQuery);
        }

        private readonly struct SeparationQuery : IForEach<Position, Separation, SeparationForce>
        {
            public void Update(ref Position position, ref Separation separation, ref SeparationForce separationForce)
            {
                var offset = separationForce.Force / (separationForce.ForcesCount == 0 ? 1 : separationForce.ForcesCount);
                position.Value += (offset.xy * separation.AffectedCoeff).x_y;
                separationForce.Force = fix2.zero;
                separationForce.ForcesCount = 0;
            }
        }
        private readonly struct SeparationForceQuery : IForEach<Position, Separation, Radius>
        {
            private readonly World _world;
            private readonly Dictionary<int2, List<Entity>> _partitioning;
            private readonly List<Entity> _targetsCache;

            public SeparationForceQuery(World world, Dictionary<int2, List<Entity>> targets, List<Entity> targetsCache)
            {
                _world = world;
                _partitioning = targets;
                _targetsCache = targetsCache;
            }

            public void Update(ref Position position, ref Separation separation, ref Radius circleShape)
            {
                FindTargets(ref position, ref separation, ref circleShape);

                fix2 posXZ = position.Value.xz;
                fix radius = circleShape.Value;
                fix addRadius = separation.AddRadius;
                fix extendedRadius = radius + addRadius;

                foreach (var other in _targetsCache)
                {
                    var otherPos = _world.Get<Position>(other).Value.xz;
                    var dir = otherPos - posXZ;
                    var sqrDist = fix2.SqrLength(dir);
                    var distance = Maths.Sqrt(sqrDist);

                    var softForce = 1 - Maths.Clamp(Maths.InvLerp(radius, extendedRadius, distance), 0, 1);
                    softForce *= softForce;
                    var hardForce = 1 - Maths.Clamp(Maths.InvLerp(0, radius, distance), 0, 1);
                    dir = sqrDist == 0 ? fix2.zero : dir / distance;
                    ref var otherSep = ref _world.Get<SeparationForce>(other);
                    otherSep.Force += dir * (hardForce + softForce) * separation.AffectingCoeff;
                    otherSep.ForcesCount++;
                }
            }


            private void FindTargets(ref Position position, ref Separation separation, ref Radius circleShape)
            {
                _targetsCache.Clear();
                var distance = circleShape.Value + separation.AddRadius;
                var pos = position.Value.xz;
                var range = new fix2(distance, distance);
                var min = GetQuantizedSquare(pos - range);
                var max = GetQuantizedSquare(pos + range);
                var sqrDistance = distance * distance;

                for (int y = min.y; y <= max.y; y++)
                {
                    for (int x = min.x; x <= max.x; x++)
                    {
                        if (!_partitioning.TryGetValue(new int2(x, y), out var quadrant))
                            continue;

                        foreach (var item in quadrant)
                        {
                            var itemPos = _world.Get<Position>(item).Value.xz;
                            if (fix2.SqrDistance(itemPos, pos) < sqrDistance)
                            {
                                _targetsCache.Add(item);
                            }
                        }
                    }
                }
            }
        }

        private readonly struct PartitioningQuery : IForEachWithEntity<Position>
        {
            private readonly Dictionary<int2, List<Entity>> _targets;

            public PartitioningQuery(Dictionary<int2, List<Entity>> targets)
            {
                _targets = targets;
            }

            public readonly void Update(Entity entity, ref Position p)
            {
                var intPos = GetQuantizedSquare(p.Value.xz);
                if (!_targets.TryGetValue(intPos, out var quadrant))
                    _targets[intPos] = quadrant = new();
                quadrant.Add(entity);
            }
        }

        private static int2 GetQuantizedSquare(fix2 position)
        {
            return new int2((int)position.x / SquareSize, (int)position.y / SquareSize);
        }
    }
}
