using Arch.Core;
using DVG.SkyPirates.Shared.Components.Framed;
using DVG.SkyPirates.Shared.Components.Runtime;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;

namespace DVG.SkyPirates.Shared.Systems
{
    public sealed class CachePositionSystem : ITickableExecutor
    {
        private QueryDescription _desc = new QueryDescription().WithAll<Position, CachePosition>().NotDisposing();
        private readonly World _world;

        public CachePositionSystem(World world)
        {
            _world = world;
        }

        public void Tick(int tick, fix deltaTime)
        {
            var query = new CachePositionQuery();
            _world.InlineQuery<CachePositionQuery, Position, CachePosition>(_desc, ref query);
        }

        private readonly struct CachePositionQuery : IForEach<Position, CachePosition>
        {
            public void Update(ref Position position, ref CachePosition cachePosition)
            {
                cachePosition.Value = position.Value;
            }
        }
    }
}
