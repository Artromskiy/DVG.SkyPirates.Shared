using Arch.Core;
using DVG.Components;
using DVG.Core.Collections;
using DVG.SkyPirates.Shared.Components.Config;
using DVG.SkyPirates.Shared.Components.Framed;
using DVG.SkyPirates.Shared.Components.Runtime;
using DVG.SkyPirates.Shared.IServices;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Systems
{
    /// <summary>
    /// Caches Entities with their position quantized by <see cref="SquareSize"/>.
    /// Use for fast nearest search
    /// </summary>
    public sealed class TargetSearchSystem : ITargetSearchSystem // Should be used before any Position/Team writes for accurate search
    {
        private readonly QueryDescription _desc = new QueryDescription().
            WithAll<RecivedDamage, Position, Team>().NotDisposing();

        private const int SquareSize = 5;

        private readonly World _world;

        // teamId -> quad -> entities
        private readonly Dictionary<int, Lookup2D<List<Entity>>> _targets = new();
        private readonly List<Entity> _targetsCache = new();

        private readonly Lookup _entitiesLookup = new();

        public TargetSearchSystem(World world)
        {
            _world = world;
        }

        public Entity? FindTarget(
            ref Position position,
            ref TargetSearchDistance searchDistance,
            ref TargetSearchPosition searchPosition,
            ref Team team)
        {
            _targetsCache.Clear();
            FindTargets(ref searchDistance, ref searchPosition, ref team, _targetsCache);

            var origin = position.Value.xz;

            Entity? best = null;
            fix bestDist = fix.MaxValue;
            int bestSyncId = int.MaxValue;

            foreach (var entity in _targetsCache)
            {
                var targetPosXZ = _world.Get<Position>(entity).Value.xz;
                var syncId = _world.Get<SyncId>(entity).Value;
                var dist = fix2.SqrDistance(targetPosXZ, origin);

                if (best == null ||
                    dist < bestDist ||
                    (dist == bestDist && syncId < bestSyncId))
                {
                    best = entity;
                    bestDist = dist;
                    bestSyncId = syncId;
                }
            }

            return best;
        }


        public void FindTargets(
            ref TargetSearchDistance searchDistance,
            ref TargetSearchPosition searchPosition,
            ref Team team,
            List<Entity> targets)
        {
            _entitiesLookup.Clear();
            var center = searchPosition.Value.xz;
            var distance = searchDistance.Value;

            var range = new fix2(distance, distance);
            var min = GetQuantizedSquare(center - range);
            var max = GetQuantizedSquare(center + range);
            var searchPositionXZ = searchPosition.Value.xz;
            var sqrSearchDistance = searchDistance.Value * searchDistance.Value;

            foreach (var kv in _targets)
            {
                if (kv.Key == team.Id)
                    continue;

                var grid = kv.Value;

                for (int y = min.y; y <= max.y; y++)
                {
                    for (int x = min.x; x <= max.x; x++)
                    {
                        if (!grid.TryGetValue(x, y, out var list))
                            continue;

                        for (int i = 0; i < list.Count; i++)
                        {
                            if (_entitiesLookup.Has(list[i].Id))
                                continue;

                            if (fix2.SqrDistance(_world.Get<Position>(list[i]).Value.xz, searchPositionXZ) < sqrSearchDistance)
                            {
                                targets.Add(list[i]);
                                _entitiesLookup.Add(list[i].Id);
                            }
                        }
                    }
                }
            }
        }

        public void Tick(int tick, fix deltaTime)
        {
            foreach (var team in _targets.Values)
                team.Clear();

            var query = new PartitionQuery(_targets);
            _world.InlineEntityQuery<PartitionQuery, Position, Team>(_desc, ref query);
        }

        private readonly struct PartitionQuery : IForEachWithEntity<Position, Team>
        {
            private readonly Dictionary<int, Lookup2D<List<Entity>>> _targets;

            public PartitionQuery(Dictionary<int, Lookup2D<List<Entity>>> targets)
            {
                _targets = targets;
            }

            public void Update(Entity e, ref Position p, ref Team t)
            {
                var quad = GetQuantizedSquare(p.Value.xz);

                if (!_targets.TryGetValue(t.Id, out var team))
                    _targets[t.Id] = team = new();

                if (!team.TryGetValue(quad.x, quad.y, out var list))
                    team[quad.x, quad.y] = list = new List<Entity>(8);

                list.Add(e);
            }
        }

        private static int2 GetQuantizedSquare(fix2 position)
        {
            return new int2(
                (int)position.x / SquareSize,
                (int)position.y / SquareSize);
        }
    }
}
