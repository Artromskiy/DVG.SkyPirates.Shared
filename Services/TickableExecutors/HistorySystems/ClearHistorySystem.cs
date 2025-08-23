using Arch.Core;
using DVG.Core;
using DVG.Core.History;
using DVG.SkyPirates.Shared.Components.Special;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;
using System.Collections.Generic;
using System;

namespace DVG.SkyPirates.Shared.Services.TickableExecutors.HistorySystems
{
    public class ClearHistorySystem : IPostTickableExecutor
    {
        private readonly Descriptions _descriptions = new Descriptions();
        private const int RemoveTicksAfter = 600;
        private readonly World _world;

        public ClearHistorySystem(World world)
        {
            _world = world;
        }

        public void Tick(int tick, fix deltaTime)
        {
            HistoryIds.ForEachData(new ClearHistoryAction(_descriptions, _world, tick));
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
        private class Description<T> : IDescription where T : struct
        {
            public readonly QueryDescription desc = new QueryDescription().WithAll<History<T>>();
        }


        private readonly struct ClearHistoryAction : IStructGenericAction
        {
            private readonly Descriptions _descriptions;
            private readonly World _world;
            private readonly int _tick;

            public ClearHistoryAction(Descriptions descriptions, World world, int tick)
            {
                _descriptions = descriptions;
                _world = world;
                _tick = tick;
            }

            public void Invoke<T>() where T : struct
            {
                var query = new ClearHistoryQuery<T>(_tick);
                var desc = _descriptions.GetDescription<T>().desc;
                _world.InlineQuery<ClearHistoryQuery<T>, History<T>>(desc, ref query);
            }
        }

        private readonly struct ClearHistoryQuery<T> : IForEach<History<T>>
            where T : struct
        {
            private readonly int _tick;

            public ClearHistoryQuery(int tick)
            {
                _tick = tick;
            }

            public readonly void Update(ref History<T> history)
            {
                history.history.Remove(_tick - RemoveTicksAfter);
            }
        }
    }
}
