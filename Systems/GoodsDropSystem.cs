using Arch.Core;
using DVG.Components;
using DVG.SkyPirates.Shared.Components.Config;
using DVG.SkyPirates.Shared.Components.Framed;
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

        private readonly List<DropInfo> _dropsData = new();
        private readonly World _world;

        public GoodsDropSystem(World world)
        {
            _world = world;
        }

        public void Tick(int tick, fix deltaTime)
        {
            _dropsData.Clear();
            var query = new CreateGoodsQuery();
            _world.InlineQuery<CreateGoodsQuery, Health, GoodsDrop, Position, SyncIdReserve, RandomSeed>(_desc, ref query);
            foreach (var item in _dropsData)
            {
                EntityParameters parameters = new(item.SyncId, default, default);
                var drop = _configedEntityFactory.Create((item.GoodsId, parameters));
                _world.Get<Position>(drop) = item.Position;
                _world.Get<Destination>(drop).Position = item.Position.Value + item.Direction.x_y;
                //_world.Get<>
            }
        }

        private readonly struct CreateGoodsQuery : IForEach<Health, GoodsDrop, Position, SyncIdReserve, RandomSeed>
        {
            private readonly List<DropInfo> _drops;
            public void Update(ref Health health, ref GoodsDrop goods, ref Position position, ref SyncIdReserve syncIdReserve, ref RandomSeed seed)
            {
                if (health.Value > 0)
                    return;

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
                            x = seed.NextRange((fix)(-1),(fix)1),
                            y = seed.NextRange((fix)(-1),(fix)1),
                        }
                    };
                    _drops.Add(drop);
                }
            }
        }

        private struct DropInfo
        {
            public Position Position;
            public GoodsId GoodsId;
            public SyncId SyncId;
            public fix2 Direction;
            public int Amount;
        }
    }
}