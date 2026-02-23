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
        public static int SquareSize = 4;

        private readonly QueryDescription _separatorDesc = new QueryDescription().
            WithAll<Position, Separator, Radius>().NotDisposing().NotDisabled();

        private readonly QueryDescription _separationDesc = new QueryDescription().
            WithAll<Position, Separation>().NotDisposing().NotDisabled();

        private readonly Lookup2D<List<SyncIdPosition>> _partitioning = new();
        private readonly Lookup<SeparationForce> _forces = new();
        private readonly Lookup _entitiesLookup = new();

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

                // we can turn problem upside down, so Separator writes to cells
                // Separation searches and updates forces applied to self
                // This can be done in parallel if force is a component

                var partitionQuery = new PartitioningQuery(_partitioning);
                _world.InlineQuery<PartitioningQuery, SyncId, Position>(_separationDesc, ref partitionQuery);

                var forceQuery = new SumSeparationForceQuery(_partitioning, _forces, _entitiesLookup);
                _world.InlineQuery<SumSeparationForceQuery, Position, Separator, Radius>(_separatorDesc, ref forceQuery);

                var separationQuery = new ApplySeparationQuery(_forces);
                _world.InlineQuery<ApplySeparationQuery, SyncId, Position, Separation>(_separationDesc, ref separationQuery);
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
                var offset = separation.Value / forcesCount * force.Force;
                position += offset.x_y;
            }
        }

        private readonly struct SumSeparationForceQuery : IForEach<Position, Separator, Radius>
        {
            private readonly Lookup2D<List<SyncIdPosition>> _partitioning;
            private readonly Lookup<SeparationForce> _forces;
            private readonly Lookup _entitiesLookup;

            public SumSeparationForceQuery(Lookup2D<List<SyncIdPosition>> partitioning, Lookup<SeparationForce> forces, Lookup entitiesLookup)
            {
                _partitioning = partitioning;
                _forces = forces;
                _entitiesLookup = entitiesLookup;
            }

            public void Update(ref Position position, ref Separator separation, ref Radius radius)
            {
                UpdateInPlace(ref position, ref separation, ref radius);
            }

            public void UpdateInPlace(ref Position position, ref Separator separator, ref Radius radius)
            {
                _entitiesLookup.Clear();

                var hardRadius = radius;
                var maxDistance = hardRadius + separator.Radius;
                var pos = ((fix3)position).xz;
                var range = new fix2(maxDistance);
                var min = pos - range;
                var max = pos + range;
                var minQ = GetQuantizedSquare(min);
                var maxQ = GetQuantizedSquare(max);
                var maxSqrDistance = maxDistance * maxDistance;

                for (int y = minQ.y; y <= maxQ.y; y++)
                {
                    for (int x = minQ.x; x <= maxQ.x; x++)
                    {
                        if (!_partitioning.TryGetValue(x, y, out var quadrant))
                            continue;

                        int count = quadrant.Count;
                        for (int i = 0; i < count; i++)
                        {
                            SyncIdPosition other = quadrant[i];
                            if (_entitiesLookup.Has(other.SyncId.Value))
                                continue;

                            var otherPos = ((fix3)other.Position).xz;

                            if (otherPos == pos)
                                continue;

                            if (otherPos.x < min.x || otherPos.x > max.x ||
                                otherPos.y < min.y || otherPos.y > max.y)
                                continue;

                            var dir = otherPos - pos;
                            var sqrDistance = fix2.SqrLength(dir);
                            if (sqrDistance >= maxSqrDistance)
                                continue;

                            _entitiesLookup.Add(other.SyncId.Value);

                            if (!_forces.TryGetValue(other.SyncId.Value, out var separationForce))
                                _forces[other.SyncId.Value] = separationForce = new SeparationForce();

                            var distance = Maths.Sqrt(sqrDistance);
                            var softForce = Maths.Clamp(Maths.InvLerp(maxDistance, hardRadius, distance), 0, 1);
                            var hardForce = Maths.Clamp(hardRadius == fix.Zero ? 0 : Maths.InvLerp(hardRadius, 0, distance), 0, 1);
                            dir = sqrDistance == 0 ? fix2.zero : dir / distance;
                            var force = Maths.Max(hardForce, softForce * softForce) * separator.Coefficient * dir;
                            _forces[other.SyncId.Value] = new()
                            {
                                Force = separationForce.Force + force,
                                ForcesCount = separationForce.ForcesCount + 1
                            };
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
