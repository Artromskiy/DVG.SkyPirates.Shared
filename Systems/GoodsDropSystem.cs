using Arch.Core;
using DVG.Components;
using DVG.SkyPirates.Shared.Components.Config;
using DVG.SkyPirates.Shared.Components.Runtime;
using DVG.SkyPirates.Shared.Data;
using DVG.SkyPirates.Shared.Ids;
using DVG.SkyPirates.Shared.IFactories;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Systems
{
    public class GoodsDropSystem : ITickableExecutor
    {
        private readonly QueryDescription _desc = new QueryDescription().
            WithAll<Health, GoodsDrop>().NotDisposing();

        private readonly IConfigedEntityFactory<GoodsId> _configedEntityFactory;
        private readonly World _world;

        private readonly List<DropInfo> _dropInfos = new();

        public GoodsDropSystem(IConfigedEntityFactory<GoodsId> configedEntityFactory, World world)
        {
            _configedEntityFactory = configedEntityFactory;
            _world = world;
        }

        public void Tick(int tick, fix deltaTime)
        {
            _dropInfos.Clear();
            var query = new CreateGoodsQuery(_dropInfos);
            _world.InlineQuery<CreateGoodsQuery, Health, GoodsDrop, Position, SyncIdReserve, RandomSeed>(_desc, ref query);
            foreach (var item in _dropInfos)
            {
                EntityParameters parameters = new(item.SyncId, default, default);
                var drop = _configedEntityFactory.Create((item.GoodsId, parameters));
                _world.Get<Position>(drop) = item.Position;
                _world.Get<Rotation>(drop) = item.Rotation;
                _world.Get<FlyDestination>(drop) = new()
                {
                    StartPosition = item.Position,
                    EndPosition = item.Position + item.Direction.x_y,
                };
                var maxSpeed = _world.Get<MaxSpeed>(drop);
                var position = _world.Get<Position>(drop);
                var flyDestination = _world.Get<FlyDestination>(drop);
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

                fix range = 3;
                int dropsCount = Maths.Min(goods.Amount, syncIdReserve.Count);
                int maxAmountPerDrop = goods.Amount / dropsCount;
                var remainingAmount = goods.Amount;
                for (int i = 0; i < dropsCount; i++)
                {
                    var dropAmount = Maths.Min(maxAmountPerDrop, remainingAmount);
                    remainingAmount -= dropAmount;
                    var drop = new DropInfo()
                    {
                        Position = position,
                        Amount = dropAmount,
                        GoodsId = goods.Id,
                        SyncId = { Value = syncIdReserve.Current++ },
                        Direction =
                        {
                            x = seed.NextRange(-range, range),
                            y = seed.NextRange(-range, range),
                        },
                        Rotation = { Value = seed.NextRange(0, 361) },
                    };
                    _dropInfos.Add(drop);
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