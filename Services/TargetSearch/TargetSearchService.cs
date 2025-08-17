using DVG.SkyPirates.Shared.IServices;
using DVG.SkyPirates.Shared.IServices.TargetSearch;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Services.TargetSearch
{
    public class TargetSearchService : ITargetSearchService
    {
        private const int SquareSize = 35;

        private readonly IEntitiesService _entitiesService;

        private readonly Dictionary<int2, Dictionary<int, List<ITarget>>> _targets = new Dictionary<int2, Dictionary<int, List<ITarget>>>();
        private readonly List<ITarget> _targetsCache = new List<ITarget>();

        public TargetSearchService(IEntitiesService entitiesService)
        {
            _entitiesService = entitiesService;
        }

        public ITarget? FindTarget(fix3 position, fix distance, int teamId)
        {
            _targetsCache.Clear();
            FindTargets(position, distance, teamId, _targetsCache);
            fix minSqrDistance = fix.MaxValue;
            ITarget? foundTarget = null;
            foreach (var target in _targetsCache)
            {
                var sqrDistance = fix2.SqrDistance(target.Position.xz, position.xz);
                if (sqrDistance < minSqrDistance)
                {
                    foundTarget = target;
                    minSqrDistance = sqrDistance;
                }
            }
            return foundTarget;
        }

        public void FindTargets(fix3 position, fix distance, int teamId, List<ITarget> targets)
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
                                if (fix2.SqrDistance(target.Position.xz, position.xz) < sqrDistance)
                                    targets.Add(target);
                }
            }
        }

        public void Tick(fix _)
        {
            foreach (var item in _targets)
                foreach (var team in item.Value)
                    team.Value.Clear();

            foreach (var entityId in _entitiesService.GetEntityIds())
            {
                if (!_entitiesService.TryGetEntity<ITarget>(entityId, out var target))
                    continue;

                var intPos = GetQuantizedSquare(target.Position.xz);
                if (!_targets.TryGetValue(intPos, out var quadrant))
                    _targets[intPos] = quadrant = new Dictionary<int, List<ITarget>>();
                if (!quadrant.TryGetValue(target.TeamId, out var team))
                    quadrant[target.TeamId] = team = new List<ITarget>();
                team.Add(target);
            }
        }

        private int2 GetQuantizedSquare(fix2 position)
        {
            var pos = position / SquareSize;
            return new int2((int)pos.x, (int)pos.y);
        }
    }
}
