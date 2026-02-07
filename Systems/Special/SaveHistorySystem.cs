using Arch.Core;
using DVG.Core;
using DVG.Core.Components;
using DVG.SkyPirates.Shared.Components.Special;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;
using DVG.SkyPirates.Shared.Tools;

namespace DVG.SkyPirates.Shared.Systems.Special
{
    internal sealed class SaveHistorySystem : ITickableExecutor
    {
        private sealed class Description<T> where T : struct
        {
            public readonly QueryDescription saveHasCmpDesc = new QueryDescription().WithAll<History<T>, T>();
            public readonly QueryDescription saveNoCmpDesc = new QueryDescription().WithAll<History<T>>().WithNone<T>();
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
                var saveQuery = new SaveHistoryQuery<T>(_tick);
                var desc = _descriptions.Get<Description<T>>();

                _world.AddQuery((ref T has, ref History<T> history) => history = History<T>.Create());

                var saveHasCmpDesc = desc.saveHasCmpDesc;
                _world.InlineQuery<SaveHistoryQuery<T>, History<T>, T>(saveHasCmpDesc, ref saveQuery);

                var saveNoCmpDesc = desc.saveNoCmpDesc;
                _world.InlineQuery<SaveHistoryQuery<T>, History<T>>(saveNoCmpDesc, ref saveQuery);
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
