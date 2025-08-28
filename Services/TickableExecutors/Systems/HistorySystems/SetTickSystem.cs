using Arch.Core;
using DVG.SkyPirates.Shared.Components.Special;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;

namespace DVG.SkyPirates.Shared.Services.TickableExecutors.Systems.HistorySystems
{
    public class SetTickSystem : ITickableExecutor
    {
        public readonly QueryDescription _desc = new QueryDescription().WithAll<TickInfo>();
        private readonly World _world;

        public SetTickSystem(World world)
        {
            _world = world;
        }

        public void Tick(int tick, fix deltaTime)
        {
            var query = new SetTickQuery(tick);
            _world.InlineQuery<SetTickQuery, TickInfo>(_desc, ref query);
        }

        private readonly struct SetTickQuery :
            IForEach<TickInfo>
        {
            private readonly int _tick;

            public SetTickQuery(int tick)
            {
                _tick = tick;
            }

            public readonly void Update(ref TickInfo tickInfo)
            {
                tickInfo.Tick = _tick;
            }
        }
    }
}
