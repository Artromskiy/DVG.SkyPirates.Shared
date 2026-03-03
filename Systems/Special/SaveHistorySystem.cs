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
                WithAll<History<T>, T>().NotDisabled().Alive();
            public readonly QueryDescription saveNoCmpDesc = new QueryDescription().
                WithAll<History<T>>().WithNone<T>().NotDisabled().Alive();

            public readonly QueryDescription saveBaselineDesc = new QueryDescription().
                WithAll<History<T>, T>().Alive();
        }

        private readonly QueryDescription _setHasAliveDesc = new QueryDescription().
            WithAll<History<Alive>, Alive>();
        private readonly QueryDescription _setNoAliveDesc = new QueryDescription().
            WithAll<History<Alive>>().WithNone<Alive>();

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

            var setHasDisposing = new SetHasHistoryQuery<Alive>(tick);
            var setNoDisposing = new SetNoHistoryQuery<Alive>(tick);

            _world.InlineQuery<SetHasHistoryQuery<Alive>, History<Alive>, Alive>
                (_setHasAliveDesc, ref setHasDisposing);
            _world.InlineQuery<SetNoHistoryQuery<Alive>, History<Alive>>
                (_setNoAliveDesc, ref setNoDisposing);
        }

        public void SaveBaseline()
        {
            var addAction = new AddHistoryAction(_world);
            HistoryComponentsRegistry.ForEachData(ref addAction);

            var saveAction = new SaveBaselineHistoryAction(_desc, _world);
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
                if (typeof(T) == typeof(Alive))
                    return;
                var desc = _descriptions.Get<Description<T>>();
                var saveHasQuery = new SetHasHistoryQuery<T>(_tick);
                var saveNoQuery = new SetNoHistoryQuery<T>(_tick);
                var saveHasCmpDesc = desc.saveHasCmpDesc;
                var saveNoCmpDesc = desc.saveNoCmpDesc;
                _world.InlineQuery<SetHasHistoryQuery<T>, History<T>, T>(in saveHasCmpDesc, ref saveHasQuery);
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


        private readonly struct SaveBaselineHistoryAction : IStructGenericAction
        {
            private readonly GenericCreator _descriptions;
            private readonly World _world;

            public SaveBaselineHistoryAction(GenericCreator descriptions, World world)
            {
                _descriptions = descriptions;
                _world = world;
            }

            public void Invoke<T>() where T : struct
            {
                if (typeof(T) == typeof(Alive))
                    return;
                var desc = _descriptions.Get<Description<T>>();
                var saveQuery = new SetBaselineHistoryQuery<T>();
                var saveDesc = desc.saveBaselineDesc;
                _world.InlineQuery<SetBaselineHistoryQuery<T>, History<T>, T>(in saveDesc, ref saveQuery);
            }
        }

        private readonly struct SetBaselineHistoryQuery<T> : IForEach<History<T>, T>
            where T : struct
        {
            public readonly void Update(ref History<T> history, ref T component)
            {
                history.Rollback(int.MinValue);
                history[int.MinValue] = component;
            }
        }

    }
}
