using Arch.Core;
using Arch.Core.Extensions;
using DVG.SkyPirates.Shared.Components;
using DVG.SkyPirates.Shared.Components.Data;
using DVG.SkyPirates.Shared.IServices;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Systems
{
    /// <summary>
    /// Caches Entities with their position quantized by <see cref="SquareSize"/>.
    /// Use for fast nearest search
    /// </summary>
    public class TargetSearchSystem : ITargetSearchSystem
    {
        private readonly QueryDescription _desc = new QueryDescription().
            WithAll<RecivedDamage, Position, Team>().
            WithNone<Dead>();

        private const int SquareSize = 2;

        private readonly World _world;

        private readonly Dictionary<int, Dictionary<int2, List<Entity>>> _targets = new Dictionary<int, Dictionary<int2, List<Entity>>>();
        private readonly List<Entity> _targetsCache = new List<Entity>();

        public TargetSearchSystem(World world)
        {
            _world = world;
        }

        public Entity FindTarget(ref TargetSearchData targetSearchData, ref Team team)
        {
            fix minSqrDistance = fix.MaxValue;
            Entity foundTarget = Entity.Null;

            _targetsCache.Clear();
            FindTargets(ref targetSearchData, ref team, _targetsCache);

            foreach (var target in _targetsCache)
            {
                var targetPosition = target.Get<Position>().Value.xz;
                var sqrDistance = fix2.SqrDistance(targetPosition, targetSearchData.Position.xz);
                if ((sqrDistance < minSqrDistance) ||
                    (sqrDistance == minSqrDistance && target.Id < foundTarget.Id))
                {
                    foundTarget = target;
                    minSqrDistance = sqrDistance;
                }
            }
            return foundTarget;
        }

        public void FindTargets(ref TargetSearchData targetSearchData, ref Team team, List<Entity> targets)
        {
            var distance = targetSearchData.Distance;
            var position = targetSearchData.Position;
            var teamId = team.Id;
            var range = new fix2(distance, distance);
            var min = GetQuantizedSquare(position.xz - range);
            var max = GetQuantizedSquare(position.xz + range);
            var sqrDistance = distance * distance;

            foreach (var t in _targets)
            {
                if (t.Key == teamId)
                    continue;

                for (int y = min.y; y <= max.y; y++)
                {
                    for (int x = min.x; x <= max.x; x++)
                    {
                        if (!t.Value.TryGetValue(new int2(x, y), out var quad))
                            continue;

                        foreach (var target in quad)
                            if (fix2.SqrDistance(target.Get<Position>().Value.xz, position.xz) < sqrDistance)
                                targets.Add(target);
                    }
                }
            }
        }

        public void Tick(int tick, fix deltaTime)
        {
            foreach (var item in _targets)
                foreach (var team in item.Value)
                    team.Value.Clear();

            var query = new TargetsPartitioningQuery(_targets);
            _world.InlineEntityQuery<TargetsPartitioningQuery, Position, Team>(_desc, ref query);
        }

        private readonly struct TargetsPartitioningQuery : IForEachWithEntity<Position, Team>
        {
            private readonly Dictionary<int, Dictionary<int2, List<Entity>>> _targets;

            public TargetsPartitioningQuery(Dictionary<int, Dictionary<int2, List<Entity>>> targets)
            {
                _targets = targets;
            }

            public readonly void Update(Entity e, ref Position p, ref Team t)
            {
                var intPos = GetQuantizedSquare(p.Value.xz);
                if (!_targets.TryGetValue(t.Id, out var team))
                    _targets[t.Id] = team = new Dictionary<int2, List<Entity>>();
                if (!team.TryGetValue(intPos, out var quad))
                    team[intPos] = quad = new List<Entity>();
                quad.Add(e);
            }
        }

        private static int2 GetQuantizedSquare(fix2 position)
        {
            return new int2((int)position.x / SquareSize, (int)position.y / SquareSize);
        }
    }
}
