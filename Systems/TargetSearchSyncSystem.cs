using Arch.Core;
using DVG.SkyPirates.Shared.Components;
using DVG.SkyPirates.Shared.Components.Data;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;

namespace DVG.SkyPirates.Shared.Systems
{
    public class TargetSearchSyncSystem : ITickableExecutor
    {
        private readonly QueryDescription _desc = new QueryDescription().WithAll<TargetSearchData, Position>();
        private readonly World _world;

        public TargetSearchSyncSystem(World world)
        {
            _world = world;
        }

        public void Tick(int tick, fix deltaTime)
        {
            var query = new SyncQuery();
            _world.InlineQuery<SyncQuery, TargetSearchData, Position>(in _desc, ref query);
        }

        private readonly struct SyncQuery : IForEach<TargetSearchData, Position>
        {
            public readonly void Update(ref TargetSearchData targetSearchData, ref Position position)
            {
                targetSearchData.Position = position.Value;
            }
        }
    }
}
