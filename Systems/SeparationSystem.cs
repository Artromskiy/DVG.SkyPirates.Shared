using Arch.Core;
using DVG.Components;
using DVG.Core.Collections;
using DVG.SkyPirates.Shared.Components.Config;
using DVG.SkyPirates.Shared.Components.Runtime;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Systems
{
    public sealed class SeparationSystem : ITickableExecutor
    {
        public static int SquareSize = 4;

        private readonly QueryDescription _separatorDesc = new QueryDescription().
            WithAll<SyncId, Position, Separator, Radius>().
            NotDisposing().NotDisabled();

        private readonly QueryDescription _separationDesc = new QueryDescription().
            WithAll<Position, Separation>().
            NotDisposing().NotDisabled();

        private readonly Lookup2D<List<SeparatorEntry>> _partitioning = new();
        private readonly HashSet<int> _syncIdCache = new();

        private readonly World _world;

        public SeparationSystem(World world)
        {
            _world = world;
        }

        public void Tick(int tick, fix deltaTime)
        {
            _partitioning.Clear();

            var partitionQuery = new PartitionQuery(_partitioning);
            _world.InlineQuery<PartitionQuery, SyncId, Position, Separator, Radius>(_separatorDesc, ref partitionQuery);

            var separationQuery = new SeparationQuery(_syncIdCache, _partitioning);
            _world.InlineQuery<SeparationQuery, Position, Separation>(_separationDesc, ref separationQuery);
        }

        private readonly struct SeparationQuery : IForEach<Position, Separation>
        {
            private readonly HashSet<int> _written;
            private readonly Lookup2D<List<SeparatorEntry>> _grid;

            public SeparationQuery(HashSet<int> written, Lookup2D<List<SeparatorEntry>> grid)
            {
                _written = written;
                _grid = grid;
            }

            public void Update(ref Position position, ref Separation separation)
            {
                _written.Clear();
                var pos = ((fix3)position).xz;

                var q = GetQuantizedSquare(pos);

                fix2 totalForce = fix2.zero;
                int forcesCount = 0;


                if (!_grid.TryGetValue(q.x, q.y, out var list))
                    return;


                int count = list.Count;
                for (int i = 0; i < count; i++)
                {
                    var other = list[i];
                    if (!_written.Add(other.SyncId))
                        continue;

                    var dir = pos - other.Position;
                    var sqrDistance = fix2.SqrLength(dir);

                    if (sqrDistance == 0)
                        continue;

                    var maxDistance = other.Radius + other.SeparatorRadius;
                    var maxSqrDistance = maxDistance * maxDistance;
                    if (sqrDistance > maxSqrDistance)
                        continue;

                    var distance = Maths.Sqrt(sqrDistance);
                    dir /= distance;

                    var softForce = Maths.Clamp(Maths.InvLerp(maxDistance, other.Radius, distance), 0, 1);

                    var hardForce = Maths.Clamp(other.Radius == fix.Zero ? 0 :
                        Maths.InvLerp(other.Radius, 0, distance), 0, 1);

                    var force = Maths.Max(hardForce, softForce * softForce) *
                        other.SeparatorCoefficient * dir;

                    totalForce += force;
                    forcesCount++;
                }

                if (forcesCount > 0)
                {
                    var offset = separation.Value / forcesCount * totalForce;
                    position += offset.x_y;
                }
            }
        }

        private readonly struct PartitionQuery : IForEach<SyncId, Position, Separator, Radius>
        {
            private readonly Lookup2D<List<SeparatorEntry>> _grid;

            public PartitionQuery(Lookup2D<List<SeparatorEntry>> grid)
            {
                _grid = grid;
            }

            public void Update(ref SyncId syncId, ref Position position, ref Separator separator, ref Radius radius)
            {
                var pos = ((fix3)position).xz;

                var range = new fix2(radius.Value);
                var minQ = GetQuantizedSquare(pos - range);
                var maxQ = GetQuantizedSquare(pos + range);

                for (int y = minQ.y; y <= maxQ.y; y++)
                {
                    for (int x = minQ.x; x <= maxQ.x; x++)
                    {
                        if (!_grid.TryGetValue(x, y, out var list))
                            _grid[x, y] = list = new List<SeparatorEntry>(8);

                        list.Add(new SeparatorEntry(syncId, position, separator, radius.Value));
                    }
                }
            }
        }

        private static int2 GetQuantizedSquare(fix2 position)
        {
            return new int2(
                (int)position.x / SquareSize,
                (int)position.y / SquareSize);
        }

        private readonly struct SeparatorEntry
        {
            public readonly SyncId SyncId;
            public readonly fix2 Position;
            public readonly fix SeparatorRadius;
            public readonly fix SeparatorCoefficient;
            public readonly fix Radius;

            public SeparatorEntry(SyncId syncId, Position position, Separator separator, Radius radius)
            {
                SyncId = syncId;
                Position = position.Value.xz;
                SeparatorRadius = separator.Radius;
                SeparatorCoefficient = separator.Coefficient;
                Radius = radius;
            }
        }
    }
}