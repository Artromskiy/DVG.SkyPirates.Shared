using Arch.Core;
using Arch.Core.Extensions;
using DVG.SkyPirates.Shared.Components;
using DVG.SkyPirates.Shared.IServices.TargetSearch;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Services.TargetSearch
{
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
            _targetsCache.Clear();
            FindTargets(position, distance, teamId, _targetsCache);
            fix minSqrDistance = fix.MaxValue;
            Entity foundTarget = Entity.Null;
            foreach (var target in _targetsCache)
            {
                var sqrDistance = fix2.SqrDistance(target.Get<Position>().position.xz, position.xz);
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

            for (int y = min.y; y < max.y; y++)
            {
                for (int x = min.x; x < max.x; x++)
                {
                    if (!_targets.TryGetValue(new int2(x, y), out var quadrant))
                        continue;

                    foreach (var item in quadrant)
                        if (item.Key != teamId)
                            foreach (var target in item.Value)
                                if (fix2.SqrDistance(target.Get<Position>().position.xz, position.xz) < sqrDistance)
                                    targets.Add(target);
                }
            }
        }

        public void Tick(fix _)
        {
            foreach (var item in _targets)
                foreach (var team in item.Value)
                    team.Value.Clear();
            var query = new QueryDescription().WithAll<Health, Position, Team>();
            _world.Query(query, (Entity e, ref Health h, ref Position p, ref Team t) =>
            {
                var intPos = GetQuantizedSquare(p.position.xz);
                if (!_targets.TryGetValue(intPos, out var quadrant))
                    _targets[intPos] = quadrant = new Dictionary<int, List<Entity>>();
                if (!quadrant.TryGetValue(t.id, out var team))
                    quadrant[t.id] = team = new List<Entity>();
                team.Add(e);
            });
        }

        private int2 GetQuantizedSquare(fix2 position)
        {
            var pos = position / SquareSize;
            return new int2((int)pos.x, (int)pos.y);
        }
    }
}
