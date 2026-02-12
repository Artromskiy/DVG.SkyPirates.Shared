using Arch.Core;
using DVG.Components;
using DVG.Core.Collections;
using DVG.SkyPirates.Shared.Components.Config;
using DVG.SkyPirates.Shared.Components.Runtime;
using DVG.SkyPirates.Shared.Ids;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Systems
{
    // crazy stuff
    public sealed class GoodsCollectorSystem : ITickableExecutor
    {
        private const int SquareSize = 5;

        private readonly World _world;

        private readonly Lookup2D<List<DropRef>> _partitioning = new();
        private readonly Lookup<BestCollector> _bestCollectors = new();
        private readonly Lookup<List<GoodsDrop>> _collectorsDrops = new();

        private readonly List<Entity> _removeDrops = new();

        private readonly QueryDescription _dropsDesc =
            new QueryDescription().WithAll<GoodsId, SyncId, Position, GoodsDrop>().
            WithNone<FlyDestination>().NotDisposing();

        // if something is collector and collectable at same time => heat death of the universe
        private readonly QueryDescription _collectorsDesc =
            new QueryDescription().WithAll<SyncId, Position, GoodsDrop, GoodsCollectorRadius>().NotDisposing();

        public GoodsCollectorSystem(World world)
        {
            _world = world;
        }

        public void Tick(int tick, fix deltaTime)
        {
            _partitioning.Clear();
            _bestCollectors.Clear();
            _collectorsDrops.Clear();
            _removeDrops.Clear();

            var partitionQuery = new PartitionQuery(_partitioning);
            _world.InlineQuery<PartitionQuery, SyncId, Position>(_dropsDesc, ref partitionQuery);

            var selectBestQuery = new SelectBestQuery(_partitioning, _bestCollectors);
            _world.InlineQuery<SelectBestQuery, SyncId, Position, GoodsCollectorRadius>(_collectorsDesc, ref selectBestQuery);

            var applyQuery = new MoveToBestQuery(_bestCollectors, _collectorsDrops, _removeDrops);
            _world.InlineEntityQuery<MoveToBestQuery, SyncId, GoodsDrop, Position, MaxSpeed>(_dropsDesc, ref applyQuery);

            var collectQuery = new CollectQuery(_collectorsDrops);
            _world.InlineQuery<CollectQuery, SyncId, GoodsDrop>(_dropsDesc, ref collectQuery);

            foreach (var item in _removeDrops)
                _world.Add<Dispose>(item);
        }

        private readonly struct PartitionQuery : IForEach<SyncId, Position>
        {
            private readonly Lookup2D<List<DropRef>> _grid;

            public PartitionQuery(Lookup2D<List<DropRef>> grid)
            {
                _grid = grid;
            }

            public void Update(ref SyncId syncId, ref Position position)
            {
                var posXZ = ((fix3)position).xz;
                var quad = GetQuantizedSquare(posXZ);

                if (!_grid.TryGetValue(quad.x, quad.y, out var list))
                    _grid[quad.x, quad.y] = list = new List<DropRef>(8);

                list.Add(new DropRef
                {
                    SyncId = syncId.Value,
                    PositionXZ = posXZ
                });
            }
        }


        private readonly struct SelectBestQuery : IForEach<SyncId, Position, GoodsCollectorRadius>
        {
            private readonly Lookup2D<List<DropRef>> _grid;
            private readonly Lookup<BestCollector> _best;

            public SelectBestQuery(
                Lookup2D<List<DropRef>> grid,
                Lookup<BestCollector> best)
            {
                _grid = grid;
                _best = best;
            }

            public void Update(ref SyncId collectorId,
                               ref Position collectorPos,
                               ref GoodsCollectorRadius radius)
            {
                fix2 center = ((fix3)collectorPos).xz;
                fix searchRadius = radius.Value;
                fix sqrSearchRadius = searchRadius * searchRadius;

                var range = new fix2(searchRadius, searchRadius);
                var min = GetQuantizedSquare(center - range);
                var max = GetQuantizedSquare(center + range);

                for (int y = min.y; y <= max.y; y++)
                {
                    for (int x = min.x; x <= max.x; x++)
                    {
                        if (!_grid.TryGetValue(x, y, out var drops))
                            continue;

                        for (int i = 0; i < drops.Count; i++)
                        {
                            var drop = drops[i];
                            var sqrDist = fix2.SqrDistance(drop.PositionXZ, center);

                            if (sqrDist > sqrSearchRadius)
                                continue;

                            int dropId = drop.SyncId;

                            if (!_best.TryGetValue(dropId, out var current))
                            {
                                _best[dropId] = new BestCollector
                                {
                                    CollectorSyncId = collectorId,
                                    SqrDistance = sqrDist,
                                    Position = collectorPos,
                                };
                            }
                            else
                            {
                                if (sqrDist < current.SqrDistance ||
                                   (sqrDist == current.SqrDistance &&
                                    collectorId.Value < current.CollectorSyncId.Value))
                                {
                                    _best[dropId] = new BestCollector
                                    {
                                        CollectorSyncId = collectorId,
                                        SqrDistance = sqrDist,
                                        Position = collectorPos,
                                    };
                                }
                            }
                        }
                    }
                }
            }
        }

        private readonly struct MoveToBestQuery : IForEachWithEntity<SyncId, GoodsDrop, Position, MaxSpeed>
        {
            private readonly Lookup<BestCollector> _best;
            private readonly Lookup<List<GoodsDrop>> _collectorsDrops;
            private readonly List<Entity> _removeDrops;

            public MoveToBestQuery(Lookup<BestCollector> best, Lookup<List<GoodsDrop>> collectorsDrops, List<Entity> removeDrops)
            {
                _best = best;
                _collectorsDrops = collectorsDrops;
                _removeDrops = removeDrops;
            }

            public void Update(Entity entity, ref SyncId dropId, ref GoodsDrop drop, ref Position position, ref MaxSpeed maxSpeed)
            {
                if (_best.TryGetValue(dropId.Value, out var best))
                {
                    position = fix3.MoveTowards(position, best.Position, maxSpeed);
                    if (fix3.SqrDistance(position, best.Position) < fix.One / 10)
                    {
                        _removeDrops.Add(entity);
                        if (!_collectorsDrops.TryGetValue(best.CollectorSyncId.Value, out var collected))
                            _collectorsDrops[best.CollectorSyncId.Value] = collected = new List<GoodsDrop>();
                        collected.Add(drop);
                    }
                }
            }
        }


        private readonly struct CollectQuery : IForEach<SyncId, GoodsDrop>
        {
            private readonly Lookup<List<GoodsDrop>> _collectorsDrops;

            public CollectQuery(Lookup<List<GoodsDrop>> collectorsDrops)
            {
                _collectorsDrops = collectorsDrops;
            }

            public void Update(ref SyncId collectoId, ref GoodsDrop drop)
            {
                if (_collectorsDrops.TryGetValue(collectoId.Value, out var toCollect))
                {
                    foreach (var item in toCollect)
                    {
                        drop.Amount += item.Amount;
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

        private struct DropRef
        {
            public int SyncId;
            public fix2 PositionXZ;
        }

        private struct BestCollector
        {
            public fix SqrDistance;
            public SyncId CollectorSyncId;
            public Position Position;
        }
    }
}
