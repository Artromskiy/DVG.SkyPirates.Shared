using Arch.Core;
using DVG.Collections;
using DVG.Components;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;

namespace DVG.SkyPirates.Shared.Systems.Special
{
    internal sealed class SaveHistorySystem : ISaveHistorySystem
    {
        private sealed class Description<T> where T : struct
        {
            public readonly QueryDescription saveHasCmpDesc = new QueryDescription().WithAll<History<T>, T>();
            public readonly QueryDescription saveNoCmpDesc = new QueryDescription().WithAll<History<T>>().WithNone<T>();
        }
        private readonly GenericCreator _desc = new();
        private readonly World _world;

        public SaveHistorySystem(World world)
        {
            _world = world;
        }

        private static int WrapTick(int tick) => Constants.WrapTick(tick);

        public void Tick(int tick, fix deltaTime)
        {
            var addAction = new AddHistoryAction(_world);
            var saveAction = new SaveHistoryAction(_desc, _world, tick);
            HistoryComponentsRegistry.ForEachData(ref addAction);
            HistoryComponentsRegistry.ForEachData(ref saveAction);
        }

        private readonly struct AddHistoryAction : IStructGenericAction
        {
            private readonly World _world;

            public AddHistoryAction(World world)
            {
                _world = world;
            }

            public void Invoke<T>() where T : struct
            {
                _world.AddQuery((ref T has, ref History<T> history) => history = new History<T>());
            }
        }

        private readonly struct SaveHistoryAction : IStructGenericAction
        {
            private readonly GenericCreator _descriptions;
            private readonly World _world;
            private readonly int _tick;

            public SaveHistoryAction(GenericCreator descriptions, World world, int tick)
            {
                _descriptions = descriptions;
                _world = world;
                _tick = tick;
            }

            public void Invoke<T>() where T : struct
            {
                var saveQuery = new SaveHistoryQuery<T>(WrapTick(_tick));
                var desc = _descriptions.Get<Description<T>>();

                // if history was inline array we could write faster
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

            public readonly void Update(ref History<T> history, ref T component) =>
                history[_tick] = component;

            public void Update(ref History<T> history) =>
                history[_tick] = null;
        }
    }
}
