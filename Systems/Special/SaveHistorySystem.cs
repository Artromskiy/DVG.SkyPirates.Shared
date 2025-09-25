using Arch.Core;
using DVG.Core;
using DVG.Core.History;
using DVG.SkyPirates.Shared.Components.Special;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;
using DVG.SkyPirates.Shared.Tools;

namespace DVG.SkyPirates.Shared.Systems.Special
{
    internal sealed class SaveHistorySystem : ITickableExecutor
    {
        private sealed class Description<T> where T : struct
        {
            public readonly QueryDescription hasDesc = new QueryDescription().WithAll<History<T>, T>();
            public readonly QueryDescription noDesc = new QueryDescription().WithAll<History<T>>().WithNone<T>();
        }
        private readonly GenericCollection _desc = new();
        private readonly World _world;

        public SaveHistorySystem(World world)
        {
            _world = world;
        }

        public void Tick(int tick, fix deltaTime)
        {
            var action = new SaveHistoryAction(_desc, _world, tick);
            HistoryIds.ForEachData(ref action);
        }

        private readonly struct SaveHistoryAction : IStructGenericAction
        {
            private readonly GenericCollection _descriptions;
            private readonly World _world;
            private readonly int _tick;

            public SaveHistoryAction(GenericCollection descriptions, World world, int tick)
            {
                _descriptions = descriptions;
                _world = world;
                _tick = tick;
            }

            public void Invoke<T>() where T : struct
            {
                var query = new SaveHistoryQuery<T>(_tick);
                var desc = _descriptions.Get<Description<T>>();

                var hasDesc = desc.hasDesc;
                _world.InlineQuery<SaveHistoryQuery<T>, History<T>, T>(hasDesc, ref query);

                var noDesc = desc.noDesc;
                _world.InlineQuery<SaveHistoryQuery<T>, History<T>>(noDesc, ref query);
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
                history[_tick] = component;
            }

            public void Update(ref History<T> history)
            {
                history[_tick] = null;
            }
        }
    }
}
