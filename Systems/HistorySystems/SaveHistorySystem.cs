using Arch.Core;
using DVG.Core;
using DVG.Core.History;
using DVG.SkyPirates.Shared.Components.Special;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;
using System;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Systems.HistorySystems
{
    internal sealed class SaveHistorySystem : ITickableExecutor
    {
        private readonly Descriptions _descriptions = new Descriptions();
        private readonly World _world;

        public SaveHistorySystem(World world)
        {
            _world = world;
        }

        public void Tick(int tick, fix deltaTime)
        {
            var action = new SaveHistoryAction(_descriptions, _world, tick);
            HistoryIds.ForEachData(ref action);
        }


        private readonly struct SaveHistoryAction : IStructGenericAction
        {
            private readonly Descriptions _descriptions;
            private readonly World _world;
            private readonly int _tick;

            public SaveHistoryAction(Descriptions descriptions, World world, int tick)
            {
                _descriptions = descriptions;
                _world = world;
                _tick = tick;
            }

            public void Invoke<T>() where T : struct
            {
                var query = new SaveHistoryQuery<T>(_tick);
                var desc = _descriptions.GetDescription<T>();

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
                history.SetValue(component, _tick);
            }

            public void Update(ref History<T> history)
            {
                history.SetValue(null, _tick);
            }
        }


        private class Descriptions
        {
            private readonly Dictionary<Type, IDescription> _descriptions = new Dictionary<Type, IDescription>();
            public Description<T> GetDescription<T>() where T : struct
            {
                var type = typeof(T);
                if (!_descriptions.TryGetValue(type, out var description))
                    _descriptions[type] = description = new Description<T>();
                return (Description<T>)description;
            }
        }
        private interface IDescription { }
        private sealed class Description<T> : IDescription where T : struct
        {
            public readonly QueryDescription hasDesc = new QueryDescription().WithAll<History<T>, T>();
            public readonly QueryDescription noDesc = new QueryDescription().WithAll<History<T>>().WithNone<T>();
        }
    }
}
