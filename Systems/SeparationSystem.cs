using Arch.Core;
using Arch.Core.Extensions;
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
    public class SeparationSystem : ITickableExecutor
    {
        private const int SquareSize = 3;

        private readonly QueryDescription _desc = new QueryDescription().
            WithAll<Position, PositionSeparation>().
            WithNone<Dead>();

        private readonly Dictionary<int2, List<(Entity, Position, PositionSeparation)>> _targets = new Dictionary<int2, List<(Entity, Position, PositionSeparation)>>();
        private readonly List<(Entity entity, Position position, PositionSeparation positionSeparation)> _targetsCache = new List<(Entity, Position, PositionSeparation)>();

        private readonly World _world;
        public SeparationSystem(World world)
        {
            _world = world;
        }

        public void Tick(int tick, fix deltaTime)
        {
            foreach (var quadrant in _targets)
                quadrant.Value.Clear();

            var partitionQuery = new PartitioningQuery(_targets);
            _world.InlineEntityQuery<PartitioningQuery, Position, PositionSeparation>(_desc, ref partitionQuery);

            var forceQuery = new SeparationForceQuery(_targets, _targetsCache);
            _world.InlineQuery<SeparationForceQuery, Position, PositionSeparation>(_desc, ref forceQuery);

            var separationQuery = new SeparationQuery(deltaTime);
            _world.InlineQuery<SeparationQuery, Position, PositionSeparation>(_desc, ref separationQuery);
        }

        private readonly struct SeparationQuery : IForEach<Position, PositionSeparation>
        {
            private readonly fix _deltaTime;

            public SeparationQuery(fix deltaTime)
            {
                _deltaTime = deltaTime;
            }

            public void Update(ref Position position, ref PositionSeparation separation)
            {
                var offset = separation.Force.xy * _deltaTime * 10;
                fix maxOffset = (fix)1 / 3;
                position.Value += fix2.SqrLength(offset) > maxOffset ?
                    fix2.Normalize(offset).x_y :
                    offset.x_y;
            }
        }
        private readonly struct SeparationForceQuery : IForEach<Position, PositionSeparation>
        {
            private readonly Dictionary<int2, List<(Entity entity, Position position, PositionSeparation)>> _targets;
            private readonly List<(Entity entity, Position position, PositionSeparation positionSeparation)> _targetsCache;

            public SeparationForceQuery(Dictionary<int2, List<(Entity, Position, PositionSeparation)>> targets, List<(Entity, Position, PositionSeparation)> targetsCache)
            {
                _targets = targets;
                _targetsCache = targetsCache;
            }

            public void Update(ref Position position, ref PositionSeparation separation)
            {
                _targetsCache.Clear();
                FindTargets(ref position, ref separation, _targetsCache);

                fix2 forceSum = fix2.zero;

                foreach (var other in _targetsCache)
                {
                    var otherPos = other.position.Value.xz;
                    var otherWeight = other.positionSeparation.Weight;
                    var sqrDist = fix2.SqrDistance(position.Value.xz, otherPos);
                    var sqrRadius = separation.Radius * separation.Radius;
                    var dir = sqrDist == 0 ? fix2.zero : fix2.Normalize(position.Value.xz - otherPos);
                    var percent = (1 - Maths.Min(1, sqrDist / sqrRadius)) * otherWeight;
                    forceSum += percent * dir;
                }
                var divisor = (_targetsCache.Count - 1) * separation.Weight;
                forceSum /= divisor == 0 ? 1 : divisor;
                separation.Force = forceSum * separation.Radius;
            }


            private void FindTargets(ref Position position, ref PositionSeparation separation, List<(Entity, Position, PositionSeparation)> targets)
            {
                var distance = separation.Radius;
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
                                targets.Add(item);
                    }
                }
            }
        }

        private readonly struct PartitioningQuery : IForEachWithEntity<Position, PositionSeparation>
        {
            private readonly Dictionary<int2, List<(Entity, Position, PositionSeparation)>> _targets;

            public PartitioningQuery(Dictionary<int2, List<(Entity, Position, PositionSeparation)>> targets)
            {
                _targets = targets;
            }

            public readonly void Update(Entity e, ref Position p, ref PositionSeparation ps)
            {
                var intPos = GetQuantizedSquare(p.Value.xz);
                if (!_targets.TryGetValue(intPos, out var quadrant))
                    _targets[intPos] = quadrant = new List<(Entity, Position, PositionSeparation)>();
                quadrant.Add((e, p, ps));
            }
        }

        private static int2 GetQuantizedSquare(fix2 position)
        {
            return new int2((int)position.x / SquareSize, (int)position.y / SquareSize);
        }
    }
}
