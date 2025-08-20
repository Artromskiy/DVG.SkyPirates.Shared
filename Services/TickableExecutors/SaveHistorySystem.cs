using Arch.Core;
using DVG.Core;
using DVG.SkyPirates.Shared.Archetypes;
using DVG.SkyPirates.Shared.Components;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;

namespace DVG.SkyPirates.Shared.Services.TickableExecutors
{
    public class SaveHistorySystem : ITickableExecutor
    {
        private readonly World _world;

        public SaveHistorySystem(World world)
        {
            _world = world;
        }

        public void Tick(int tick, fix deltaTime)
        {
            HistoryArch.ForEachData(new SaveHistoryAction(_world, tick));
        }


        private readonly struct SaveHistoryAction : IGenericAction
        {
            private readonly World _world;
            private readonly int _tick;

            public SaveHistoryAction(World world, int tick)
            {
                _world = world;
                _tick = tick;
            }

            public void Invoke<T>()
            {
                var query = new SaveHistoryQuery<T>(_tick);
                _world.InlineQuery<SaveHistoryQuery<T>, T, History<T>>
                    (HistoryArch.Query<T>(), ref query);
            }
        }

        private readonly struct SaveHistoryQuery<T> : IForEach<T, History<T>>
        {
            private readonly int _tick;

            public SaveHistoryQuery(int tick)
            {
                _tick = tick;
            }

            public readonly void Update(ref T component, ref History<T> history)
            {
                history.history[_tick] = component;
            }
        }
    }
}
