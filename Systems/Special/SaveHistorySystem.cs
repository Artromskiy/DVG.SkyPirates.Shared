using Arch.Core;
using DVG.Collections;
using DVG.Components;

namespace DVG.SkyPirates.Shared.Systems.Special
{
    internal sealed class SaveHistorySystem
    {
        private sealed class Description<T> where T : struct
        {
            public readonly QueryDescription saveHasCmpDesc = new QueryDescription().
                WithAll<History<T>, T>().NotDisabled();
            public readonly QueryDescription saveNoCmpDesc = new QueryDescription().
                WithAll<History<T>>().WithNone<T>().NotDisabled();
        }

        private readonly GenericCreator _desc = new();
        private readonly World _world;

        public SaveHistorySystem(World world)
        {
            _world = world;
        }

        public void Save(int tick)
        {
            var addAction = new AddHistoryAction(_world);
            HistoryComponentsRegistry.ForEachData(ref addAction);

            var saveAction = new SaveHistoryAction(_desc, _world, tick);
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
                _world.AddQuery((ref T has, ref History<T> history) =>
                    history = new History<T>(Constants.MaxHistoryTicks));
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
                var desc = _descriptions.Get<Description<T>>();
                var saveHasQuery = new SetHasHistoryQuery<T>(_tick);
                var saveNoQuery = new SetNoHistoryQuery<T>(_tick);
                var saveHasCmpDesc = desc.saveHasCmpDesc;
                _world.InlineQuery<SetHasHistoryQuery<T>, History<T>, T>(in saveHasCmpDesc, ref saveHasQuery);
                var saveNoCmpDesc = desc.saveNoCmpDesc;
                _world.InlineQuery<SetNoHistoryQuery<T>, History<T>>(in saveNoCmpDesc, ref saveNoQuery);
            }
        }

        private readonly struct SetHasHistoryQuery<T> : IForEach<History<T>, T>
            where T : struct
        {
            private readonly int _tick;
            public SetHasHistoryQuery(int tick) => _tick = tick;

            public readonly void Update(ref History<T> history, ref T component) =>
                history[_tick] = component;
        }

        private readonly struct SetNoHistoryQuery<T> : IForEach<History<T>>
            where T : struct
        {
            private readonly int _tick;
            public SetNoHistoryQuery(int tick) => _tick = tick;

            public void Update(ref History<T> history) =>
                history[_tick] = null;
        }
    }
}
