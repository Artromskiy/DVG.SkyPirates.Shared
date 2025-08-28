using Arch.Core;
using Arch.Core.Extensions;
using DVG.SkyPirates.Shared.Components;
using DVG.SkyPirates.Shared.Components.Data;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Services.TickableExecutors.Systems
{
    /// <summary>
    /// Moves Entity's <see href="Position"/> and <see href="Rotation"/> 
    /// with speed <see href="MoveSpeed"/> towards <see href="Destination"/>
    /// </summary>
    public class SeparationSystem : ITickableExecutor
    {
        private const int SquareSize = 3;
        private readonly QueryDescription _desc = new QueryDescription().WithAll<Position, PositionSeparation>();
        private readonly Dictionary<int2, List<Entity>> _targets = new Dictionary<int2, List<Entity>>();
        private readonly List<Entity> _targetsCache = new List<Entity>();

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
            _world.InlineEntityQuery<PartitioningQuery, Position>(_desc, ref partitionQuery);

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
                position.Value = separation.Force.x_y * _deltaTime;
            }
        }
        private readonly struct SeparationForceQuery : IForEach<Position, PositionSeparation>
        {
            private readonly Dictionary<int2, List<Entity>> _targets;
            private readonly List<Entity> _targetsCache;

            public SeparationForceQuery(Dictionary<int2, List<Entity>> targets, List<Entity> targetsCache)
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
                    var otherPos = other.Get<Position>().Value.xz;
                    var otherWeight = other.Get<PositionSeparation>().Weight;
                    var dist = fix2.Distance(position.Value.xz, otherPos);
                    var percent = (1 - Maths.Min(1, dist / separation.Radius)) * otherWeight;
                    forceSum += percent;
                }
                var multiplier = (_targetsCache.Count - 1) * separation.Weight;
                forceSum /= (multiplier == 0 ? 1 : multiplier);
                separation.Force = forceSum;
            }


            private void FindTargets(ref Position position, ref PositionSeparation separation, List<Entity> targets)
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
                            if (fix2.SqrDistance(item.Get<Position>().Value.xz, pos) < sqrDistance)
                                targets.Add(item);
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

            public readonly void Update(Entity e, ref Position p)
            {
                var intPos = GetQuantizedSquare(p.Value.xz);
                if (!_targets.TryGetValue(intPos, out var quadrant))
                    _targets[intPos] = quadrant = new List<Entity>();
                quadrant.Add(e);
            }
        }

        private static int2 GetQuantizedSquare(fix2 position)
        {
            return new int2((int)position.x / SquareSize, (int)position.y / SquareSize);
        }
    }
}
