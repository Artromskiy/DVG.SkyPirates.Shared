using Arch.Core;
using DVG.SkyPirates.Shared.Components.Config;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;

namespace DVG.SkyPirates.Shared.Systems
{
    public class ComponentRefCopySystem : ITickableExecutor
    {
        private readonly World _world;
        private readonly QueryDescription _goodsDesc = new QueryDescription().WithAll<GoodsDrop>();

        public ComponentRefCopySystem(World world)
        {
            _world = world;
        }

        public void Tick(int tick, fix deltaTime)
        {
            var goodsQuery = new CopyGoodsDrops();
            _world.InlineQuery<CopyGoodsDrops, GoodsDrop>(in _goodsDesc, ref goodsQuery);
        }

        private readonly struct CopyGoodsDrops : IForEach<GoodsDrop>
        {
            public readonly void Update(ref GoodsDrop goods)
            {
                var old = goods.Values;
                goods.Values = new(old);
            }
        }
    }
}
