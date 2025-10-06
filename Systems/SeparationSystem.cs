using Arch.Core;
using DVG.SkyPirates.Shared.Components;
using DVG.SkyPirates.Shared.Components.Data;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;
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
            WithAll<Position, Separation, Alive>();
        private readonly QueryDescription _affectingDesc = new QueryDescription().
            WithAll<Position, Separation, CircleShape, Alive>();

        private readonly Dictionary<int2, List<(EntityData, Position, Separation)>> _partitioning = new();
        private readonly List<(EntityData, Position, Separation)> _targetsCache = new();

        private readonly World _world;
        public SeparationSystem(World world)
        {
            _world = world;
        }

        public void Tick(int tick, fix deltaTime)
        {
            foreach (var quadrant in _partitioning)
                quadrant.Value.Clear();

            var partitionQuery = new PartitioningQuery(_world, _partitioning);
            _world.InlineEntityQuery<PartitioningQuery, Position, Separation>(_affectedDesc, ref partitionQuery);

            var forceQuery = new SeparationForceQuery(_partitioning, _targetsCache);
            _world.InlineQuery<SeparationForceQuery, Position, Separation, CircleShape>(_affectingDesc, ref forceQuery);

            var separationQuery = new SeparationQuery(deltaTime);
            _world.InlineQuery<SeparationQuery, Position, Separation>(_affectedDesc, ref separationQuery);
        }

        private readonly struct SeparationQuery : IForEach<Position, Separation>
        {
            public SeparationQuery(fix deltaTime)
            {

            }

            public void Update(ref Position position, ref Separation separation)
            {
                var offset = separation.Force / (separation.ForcesCount == 0 ? 1 : separation.ForcesCount);
                position.Value += offset.x_y * separation.AffectedCoeff;
                separation.Force = fix2.zero;
                separation.ForcesCount = 0;
            }
        }
        private readonly struct SeparationForceQuery : IForEach<Position, Separation, CircleShape>
        {
            private readonly Dictionary<int2, List<(EntityData, Position position, Separation)>> _targets;
            private readonly List<(EntityData entityData, Position position, Separation separation)> _targetsCache;

            public SeparationForceQuery(Dictionary<int2, List<(EntityData, Position, Separation)>> targets, List<(EntityData, Position, Separation)> targetsCache)
            {
                _targets = targets;
                _targetsCache = targetsCache;
            }

            public void Update(ref Position position, ref Separation separation, ref CircleShape circleShape)
            {
                FindTargets(ref position, ref separation, ref circleShape);

                fix2 posXZ = position.Value.xz;
                fix radius = circleShape.Radius;
                fix addRadius = separation.AddRadius; 
                fix extendedRadius = radius + addRadius;

                foreach (var other in _targetsCache)
                {
                    var otherPos = other.position.Value.xz;
                    var dir = otherPos - posXZ;
                    var sqrDist = fix2.SqrLength(dir);
                    var distance = Maths.Sqrt(sqrDist);

                    var softForce = 1 - Maths.Clamp(Maths.InvLerp(radius, extendedRadius, distance), 0, 1);
                    softForce *= softForce;
                    var hardForce = 1 - Maths.Clamp(Maths.InvLerp(0, radius, distance), 0, 1);
                    dir = sqrDist == 0 ? fix2.zero : dir / distance;
                    ref var otherSep = ref other.entityData.Get<Separation>();
                    otherSep.Force += dir * (hardForce + softForce) * separation.AffectingCoeff;
                    otherSep.ForcesCount++;
                }
            }


            private void FindTargets(ref Position position, ref Separation separation, ref CircleShape circleShape)
            {
                _targetsCache.Clear();
                var distance = circleShape.Radius + separation.AddRadius;
                var pos = position.Value.xz;
                var range = new fix2(distance, distance);
                var min = GetQuantizedSquare(pos - range);
                var max = GetQuantizedSquare(pos + range);
                var sqrDistance = distance * distance;

                for (int y = min.y; y <= max.y; y++)
                {
                    for (int x = min.x; x <= max.x; x++)
                    {
                        if (!_targets.TryGetValue(new int2(x, y), out var quadrant))
                            continue;

                        foreach (var item in quadrant)
                            if (fix2.SqrDistance(item.position.Value.xz, pos) < sqrDistance)
                                _targetsCache.Add(item);
                    }
                }
            }
        }

        private readonly struct PartitioningQuery : IForEachWithEntity<Position, Separation>
        {
            private readonly World _world;
            private readonly Dictionary<int2, List<(EntityData, Position, Separation)>> _targets;

            public PartitioningQuery(World world, Dictionary<int2, List<(EntityData, Position, Separation)>> targets)
            {
                _world = world;
                _targets = targets;
            }

            public readonly void Update(Entity entity, ref Position p, ref Separation ps)
            {
                var intPos = GetQuantizedSquare(p.Value.xz);
                if (!_targets.TryGetValue(intPos, out var quadrant))
                    _targets[intPos] = quadrant = new();
                quadrant.Add((_world.GetEntityData(entity), p, ps));
            }
        }

        private static int2 GetQuantizedSquare(fix2 position)
        {
            return new int2((int)position.x / SquareSize, (int)position.y / SquareSize);
        }
    }
}
