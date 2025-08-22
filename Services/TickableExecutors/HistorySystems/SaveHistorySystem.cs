using Arch.Core;
using DVG.Core;
using DVG.SkyPirates.Shared.Archetypes;
using DVG.SkyPirates.Shared.Components.Special;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;

namespace DVG.SkyPirates.Shared.Services.TickableExecutors.HistorySystems
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


        private readonly struct SaveHistoryAction : IStructGenericAction
        {
            private readonly World _world;
            private readonly int _tick;

            public SaveHistoryAction(World world, int tick)
            {
                _world = world;
                _tick = tick;
            }

            public void Invoke<T>() where T : struct
            {
                var query = new SaveHistoryQuery<T>(_tick);

                _world.InlineQuery<SaveHistoryQuery<T>, History<T>, T>
                    (new QueryDescription().WithAll<History<T>, T>(), ref query);

                _world.InlineQuery<SaveHistoryQuery<T>, History<T>>
                    (new QueryDescription().WithAll<History<T>>().WithNone<T>(), ref query);
            }
        }

        private readonly struct SaveHistoryQuery<T> :
            IForEach<History<T>, T>,
            IForEach<History<T>>
            where T : struct
        {
            private readonly int _tick;

            public SaveHistoryQuery(int tick)
            {
                _tick = tick;
            }

            public readonly void Update(ref History<T> history, ref T component)
            {
                history.history[_tick] = component;
            }

            public void Update(ref History<T> history)
            {
                history.history[_tick] = null;
            }
        }
    }
}
