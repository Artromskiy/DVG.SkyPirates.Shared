using Arch.Core;
using Arch.Core.Extensions;
using DVG.SkyPirates.Shared.Archetypes;
using DVG.SkyPirates.Shared.Components.Data;
using DVG.SkyPirates.Shared.IServices.TargetSearch;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Services.TargetSearch
{
    /// <summary>
    /// Caches Entities with their position quantized by <see cref="SquareSize"/>.
    /// Use for fast nearest search
    /// </summary>
    public class TargetSearchService : ITargetSearchService
    {
        private const int SquareSize = 35;

        private readonly World _world;

        private readonly Dictionary<int2, Dictionary<int, List<Entity>>> _targets = new Dictionary<int2, Dictionary<int, List<Entity>>>();
        private readonly List<Entity> _targetsCache = new List<Entity>();

        public TargetSearchService(World world)
        {
            _world = world;
        }

        public Entity FindTarget(fix3 position, fix distance, int teamId)
        {
            fix minSqrDistance = fix.MaxValue;
            Entity foundTarget = Entity.Null;

            _targetsCache.Clear();
            FindTargets(position, distance, teamId, _targetsCache);

            foreach (var target in _targetsCache)
            {
                var targetPosition = target.Get<Position>().Value.xz;
                var sqrDistance = fix2.SqrDistance(targetPosition, position.xz);
                if (sqrDistance < minSqrDistance)
                {
                    foundTarget = target;
                    minSqrDistance = sqrDistance;
                }
            }
            return foundTarget;
        }

        public void FindTargets(fix3 position, fix distance, int teamId, List<Entity> targets)
        {
            var d = new fix2(distance, distance);
            var min = GetQuantizedSquare(position.xz - d);
            var max = GetQuantizedSquare(position.xz + d);
            var sqrDistance = distance * distance;

            for (int y = min.y; y <= max.y; y++)
            {
                for (int x = min.x; x <= max.x; x++)
                {
                    if (!_targets.TryGetValue(new int2(x, y), out var quadrant))
                        continue;

                    foreach (var item in quadrant)
                        if (item.Key != teamId)
                            foreach (var target in item.Value)
                                if (fix2.SqrDistance(target.Get<Position>().Value.xz, position.xz) < sqrDistance)
                                    targets.Add(target);
                }
            }
        }

        public void Tick(int tick, fix deltaTime)
        {
            foreach (var item in _targets)
                foreach (var team in item.Value)
                    team.Value.Clear();

            var query = new TargetsPartitioningQuery(_targets);
            _world.InlineEntityQuery<TargetsPartitioningQuery, Position, Team>(new TargetArch(), ref query);
        }

        private readonly struct TargetsPartitioningQuery : IForEachWithEntity<Position, Team>
        {
            private readonly Dictionary<int2, Dictionary<int, List<Entity>>> _targets;

            public TargetsPartitioningQuery(Dictionary<int2, Dictionary<int, List<Entity>>> targets)
            {
                _targets = targets;
            }

            public readonly void Update(Entity e, ref Position p, ref Team t)
            {
                var intPos = GetQuantizedSquare(p.Value.xz);
                if (!_targets.TryGetValue(intPos, out var quadrant))
                    _targets[intPos] = quadrant = new Dictionary<int, List<Entity>>();
                if (!quadrant.TryGetValue(t.Id, out var team))
                    quadrant[t.Id] = team = new List<Entity>();
                team.Add(e);
            }
        }

        private static int2 GetQuantizedSquare(fix2 position)
        {
            return new int2((int)position.x / SquareSize, (int)position.y / SquareSize);
        }
    }
}
