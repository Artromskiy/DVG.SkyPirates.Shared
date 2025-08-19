using Arch.Core;
using DVG.Core;
using DVG.SkyPirates.Shared.Archetypes;
using DVG.SkyPirates.Shared.Components;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;

namespace DVG.SkyPirates.Shared.Services.TickableExecutors
{
    public class ApplyHistorySystem : IPreTickableExecutor
    {
        private readonly World _world;

        public ApplyHistorySystem(World world)
        {
            _world = world;
        }

        public void Tick(int tick, fix deltaTime)
        {
            HistoryArch.ForEachData(new ApplyHistoryAction(_world, tick));
        }


        private readonly struct ApplyHistoryAction : IGenericAction
        {
            private readonly World _world;
            private readonly int _tickToGo;

            public ApplyHistoryAction(World world, int tick)
            {
                _world = world;
                _tickToGo = tick;
            }

            public void Invoke<T>()
            {
                var query = new ApplyHistoryQuery<T>(_tickToGo);
                _world.InlineQuery<ApplyHistoryQuery<T>, T, History<T>>
                    (HistoryArch.Query<T>(), ref query);
            }
        }

        private readonly struct ApplyHistoryQuery<T> : IForEach<T, History<T>>
        {
            private readonly int _tick;

            public ApplyHistoryQuery(int tick)
            {
                _tick = tick;
            }

            public readonly void Update(ref T component, ref History<T> history)
            {
                component = history.history[_tick];
            }
        }
    }
}
