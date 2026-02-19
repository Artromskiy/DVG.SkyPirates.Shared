using Arch.Core;
using DVG.SkyPirates.Shared.Components.Framed;
using DVG.SkyPirates.Shared.Components.Runtime;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;

namespace DVG.SkyPirates.Shared.Systems
{
    public class SearchPositionSyncSystem : ITickableExecutor
    {
        private readonly QueryDescription _desc = new QueryDescription().WithAll<TargetSearchPosition, Position>().NotDisposing().NotDisabled();
        private readonly World _world;

        public SearchPositionSyncSystem(World world)
        {
            _world = world;
        }

        public void Tick(int tick, fix deltaTime)
        {
            var query = new SyncQuery();
            _world.InlineQuery<SyncQuery, TargetSearchPosition, Position>(in _desc, ref query);
        }

        private readonly struct SyncQuery : IForEach<TargetSearchPosition, Position>
        {
            public readonly void Update(ref TargetSearchPosition searchPosition, ref Position position)
            {
                searchPosition = (fix3)position;
            }
        }
    }
}
