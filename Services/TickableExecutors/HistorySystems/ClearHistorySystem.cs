using Arch.Core;
using DVG.Core;
using DVG.SkyPirates.Shared.Archetypes;
using DVG.SkyPirates.Shared.Components.Special;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;

namespace DVG.SkyPirates.Shared.Services.TickableExecutors.HistorySystems
{
    public class ClearHistorySystem : IPostTickableExecutor
    {
        private const int RemoveTicksAfter = 600;
        private readonly World _world;

        public ClearHistorySystem(World world)
        {
            _world = world;
        }

        public void Tick(int tick, fix deltaTime)
        {
            HistoryArch.ForEachData(new ClearHistoryAction(_world, tick));
        }

        private readonly struct ClearHistoryAction : IStructGenericAction
        {
            private readonly World _world;
            private readonly int _tick;

            public ClearHistoryAction(World world, int tick)
            {
                _world = world;
                _tick = tick;
            }

            public void Invoke<T>() where T : struct
            {
                var query = new ClearHistoryQuery<T>(_tick);
                _world.InlineQuery<ClearHistoryQuery<T>, T, History<T>>
                    (HistoryArch.Query<T>(), ref query);
            }
        }

        private readonly struct ClearHistoryQuery<T> : IForEach<T, History<T>>
            where T : struct
        {
            private readonly int _tick;

            public ClearHistoryQuery(int tick)
            {
                _tick = tick;
            }

            public readonly void Update(ref T component, ref History<T> history)
            {
                history.history.Remove(_tick - RemoveTicksAfter);
            }
        }
    }
}
