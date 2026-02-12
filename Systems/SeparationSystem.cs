using Arch.Core;
using DVG.Components;
using DVG.Core.Collections;
using DVG.SkyPirates.Shared.Components.Config;
using DVG.SkyPirates.Shared.Components.Runtime;
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
        private const int SquareSize = 4;

        private readonly QueryDescription _separatorDesc = new QueryDescription().
            WithAll<Position, Separator>().NotDisposing();

        private readonly QueryDescription _separationDesc = new QueryDescription().
            WithAll<Position, Separator, Radius>().NotDisposing();

        private readonly Lookup2D<List<SyncIdPosition>> _partitioning = new();
        private readonly Lookup<SeparationForce> _forces = new();
        private readonly Lookup _entitiesLookup = new();

        private readonly List<SyncIdPosition> _targetsCache = new();

        private readonly World _world;
        public SeparationSystem(World world)
        {
            _world = world;
        }

        public void Tick(int tick, fix deltaTime)
        {
            for (int i = 0; i < 2; i++)
            {
                _forces.Clear();
                _partitioning.Clear();

                var partitionQuery = new PartitioningQuery(_partitioning);
                _world.InlineQuery<PartitioningQuery, SyncId, Position>(_separatorDesc, ref partitionQuery);

                var forceQuery = new SumSeparationForceQuery(_partitioning, _forces, _entitiesLookup, _targetsCache);
                _world.InlineQuery<SumSeparationForceQuery, Position, Separator, Radius>(_separationDesc, ref forceQuery);

                var separationQuery = new ApplySeparationQuery(_forces);
                _world.InlineQuery<ApplySeparationQuery, SyncId, Position, Separation>(_separatorDesc, ref separationQuery);
            }
        }

        private readonly struct ApplySeparationQuery : IForEach<SyncId, Position, Separation>
        {
            private readonly Lookup<SeparationForce> _forces;

            public ApplySeparationQuery(Lookup<SeparationForce> forces)
            {
                _forces = forces;
            }

            public readonly void Update(ref SyncId syncId, ref Position position, ref Separation separation)
            {
                _forces.TryGetValue(syncId.Value, out var force);
                var forcesCount = force.ForcesCount == 0 ? 1 : force.ForcesCount;
                var offset = force.Force * (separation.Value / forcesCount);
                position += offset.x_y;
            }
        }

        private readonly struct SumSeparationForceQuery : IForEach<Position, Separator, Radius>
        {
            private readonly Lookup2D<List<SyncIdPosition>> _partitioning;
            private readonly Lookup<SeparationForce> _forces;
            private readonly Lookup _entititesLookup;
            private readonly List<SyncIdPosition> _targetsCache;

            public SumSeparationForceQuery(Lookup2D<List<SyncIdPosition>> partitioning, Lookup<SeparationForce> forces, Lookup entititesLookup, List<SyncIdPosition> targetsCache)
            {
                _partitioning = partitioning;
                _forces = forces;
                _entititesLookup = entititesLookup;
                _targetsCache = targetsCache;
            }

            public void Update(ref Position position, ref Separator separation, ref Radius circleShape)
            {
                FindTargets(ref position, ref separation, ref circleShape);

                fix2 posXZ = ((fix3)position).xz;
                fix radius = circleShape;
                fix addRadius = separation.Radius;
                fix extendedRadius = radius + addRadius;

                foreach (var other in _targetsCache)
                {
                    var otherPos = ((fix3)other.Position).xz;
                    if (!_forces.TryGetValue(other.SyncId.Value, out var separationForce))
                        _forces[other.SyncId.Value] = separationForce = new SeparationForce();

                    var dir = otherPos - posXZ;
                    var sqrDist = fix2.SqrLength(dir);
                    var distance = Maths.Sqrt(sqrDist);

                    var softForce = 1 - Maths.Clamp(Maths.InvLerp(radius, extendedRadius, distance), 0, 1);
                    softForce *= softForce;
                    var hardForce = 1 - Maths.Clamp(Maths.InvLerp(0, radius, distance), 0, 1);
                    dir = sqrDist == 0 ? fix2.zero : dir / distance;
                    var force = dir * (hardForce + softForce) * separation.Coefficient;
                    _forces[other.SyncId.Value] = new()
                    {
                        Force = separationForce.Force + force,
                        ForcesCount = separationForce.ForcesCount + 1
                    };
                }
            }


            private void FindTargets(ref Position position, ref Separator separation, ref Radius radius)
            {
                _entititesLookup.Clear();
                _targetsCache.Clear();

                var distance = radius + separation.Radius;
                var pos = ((fix3)position).xz;
                var range = new fix2(distance, distance);
                var min = GetQuantizedSquare(pos - range);
                var max = GetQuantizedSquare(pos + range);
                var sqrDistance = distance * distance;

                for (int y = min.y; y <= max.y; y++)
                {
                    for (int x = min.x; x <= max.x; x++)
                    {
                        if (!_partitioning.TryGetValue(x, y, out var quadrant))
                            continue;

                        for (int i = 0; i < quadrant.Count; i++)
                        {
                            SyncIdPosition item = quadrant[i];
                            if (_entititesLookup.Has(item.SyncId.Value))
                                continue;

                            var itemPos = ((fix3)item.Position).xz;
                            if (fix2.SqrDistance(itemPos, pos) < sqrDistance)
                            {
                                _targetsCache.Add(item);
                                _entititesLookup.Add(item.SyncId.Value);
                            }
                        }
                    }
                }
            }
        }

        private readonly struct PartitioningQuery : IForEach<SyncId, Position>
        {
            private readonly Lookup2D<List<SyncIdPosition>> _targets;

            public PartitioningQuery(Lookup2D<List<SyncIdPosition>> targets)
            {
                _targets = targets;
            }

            public readonly void Update(ref SyncId syncId, ref Position position)
            {
                var intPos = GetQuantizedSquare(((fix3)position).xz);
                if (!_targets.TryGetValue(intPos.x, intPos.y, out var quadrant))
                    _targets[intPos.x, intPos.y] = quadrant = new();
                quadrant.Add(new(syncId, position));
            }
        }

        private static int2 GetQuantizedSquare(fix2 position)
        {
            return new int2((int)position.x / SquareSize, (int)position.y / SquareSize);
        }

        private readonly struct SyncIdPosition
        {
            public readonly SyncId SyncId;
            public readonly Position Position;

            public SyncIdPosition(SyncId syncId, Position position)
            {
                SyncId = syncId;
                Position = position;
            }
        }

        private struct SeparationForce
        {
            public fix2 Force;
            public int ForcesCount;
        }
    }
}
