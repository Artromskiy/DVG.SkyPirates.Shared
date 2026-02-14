using Arch.Core;
using DVG.Components;
using DVG.SkyPirates.Shared.Components.Config;
using DVG.SkyPirates.Shared.Components.Runtime;
using DVG.SkyPirates.Shared.Data;
using DVG.SkyPirates.Shared.Ids;
using DVG.SkyPirates.Shared.IFactories;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;
using System.Collections.Generic;
using System.Diagnostics;

namespace DVG.SkyPirates.Shared.Systems
{
    public class GoodsDropSystem : ITickableExecutor
    {
        private readonly QueryDescription _removeDesc = new QueryDescription().
            WithAll<GoodsDrop>().NotDisposing();

        private readonly QueryDescription _createDesc = new QueryDescription().
            WithAll<Health, GoodsDrop>().NotDisposing();

        private readonly IConfigedEntityFactory<GoodsId> _configedEntityFactory;
        private readonly World _world;

        private readonly List<DropInfo> _dropInfos = new();
        private readonly SortedList<GoodsId, int> _goodsRemoveCache = new();

        public GoodsDropSystem(IConfigedEntityFactory<GoodsId> configedEntityFactory, World world)
        {
            _configedEntityFactory = configedEntityFactory;
            _world = world;
        }

        public void Tick(int tick, fix deltaTime)
        {
            _dropInfos.Clear();

            var removeEmptyQuery = new RemoveEmptyGoodsQuery(_goodsRemoveCache);
            _world.InlineQuery<RemoveEmptyGoodsQuery, GoodsDrop>(in _removeDesc, ref removeEmptyQuery);
            var createQuery = new CreateGoodsQuery(_dropInfos);
            _world.InlineQuery<CreateGoodsQuery, Health, GoodsDrop, Position, SyncIdReserve, RandomSeed>(_createDesc, ref createQuery);
            foreach (var item in _dropInfos)
            {
                EntityParameters parameters = new(item.SyncId, default, default);
                var drop = _configedEntityFactory.Create((item.GoodsId, parameters));
                _world.Get<Position>(drop) = item.Position;
                _world.Get<Rotation>(drop) = item.Rotation;
                _world.AddOrGet<GoodsAmount>(drop) = item.Amount;

                _world.AddOrGet<FlyDestination>(drop) = new()
                {
                    StartPosition = item.Position,
                    EndPosition = item.Position + item.Direction.x_y,
                };
            }
        }

        private readonly struct RemoveEmptyGoodsQuery : IForEach<GoodsDrop>
        {
            private readonly SortedList<GoodsId, int> _goodsCache;

            public RemoveEmptyGoodsQuery(SortedList<GoodsId, int> goodsCache)
            {
                _goodsCache = goodsCache;
            }

            public void Update(ref GoodsDrop goods)
            {
                _goodsCache.Clear();

                foreach (var item in goods.Values)
                    if (item.Value > 0)
                        _goodsCache.Add(item.Key, item.Value);

                if (goods.Values.Count == _goodsCache.Count)
                    return;

                goods.Values.Clear();
                foreach (var item in _goodsCache)
                    goods.Values.Add(item.Key, item.Value);
            }
        }

        private readonly struct CreateGoodsQuery : IForEach<Health, GoodsDrop, Position, SyncIdReserve, RandomSeed>
        {
            private readonly List<DropInfo> _dropInfos;

            public CreateGoodsQuery(List<DropInfo> dropInfos)
            {
                _dropInfos = dropInfos;
            }

            public void Update(ref Health health, ref GoodsDrop goods, ref Position position, ref SyncIdReserve syncIdReserve, ref RandomSeed seed)
            {
                if (health > fix.Zero)
                    return;

                int remainingIds = syncIdReserve.RemainingCount();

                int typesCount = goods.Values.Count;

                if (typesCount == 0)
                    return;

                Debug.Assert(remainingIds >= typesCount);
                if (remainingIds < typesCount)
                    return;

                int dropsCount = remainingIds;

                int baseSlots = dropsCount / typesCount;
                int remainderSlots = dropsCount % typesCount;

                int range = 4;

                for (int i = 0; i < goods.Values.Count; i++)
                {
                    var goodsId = goods.Values.Keys[i];
                    int totalAmount = goods.Values.Values[i];

                    int slots = baseSlots + (i < remainderSlots ? 1 : 0);

                    int baseAmount = totalAmount / slots;
                    int remainderAmount = totalAmount % slots;

                    for (int j = 0; j < slots; j++)
                    {
                        int amount = baseAmount + (j < remainderAmount ? 1 : 0);

                        var drop = new DropInfo
                        {
                            Position = position,
                            GoodsId = goodsId,
                            Amount = amount,
                            SyncId = syncIdReserve.GetNext(),
                            Direction =
                            {
                                x = seed.NextFixRange(-range, range),
                                y = seed.NextFixRange(-range, range),
                            },
                            Rotation = seed.NextFixRange(0, 360)
                        };

                        _dropInfos.Add(drop);
                    }
                }
            }

        }

        private struct DropInfo
        {
            public Position Position;
            public Rotation Rotation;
            public GoodsId GoodsId;
            public SyncId SyncId;
            public fix2 Direction;
            public int Amount;
        }
    }
}