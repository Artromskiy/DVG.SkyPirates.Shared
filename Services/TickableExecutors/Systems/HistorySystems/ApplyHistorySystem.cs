using Arch.Core;
using DVG.Core;
using DVG.Core.History;
using DVG.SkyPirates.Shared.Archetypes;
using DVG.SkyPirates.Shared.Components.Special;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace DVG.SkyPirates.Shared.Services.TickableExecutors.Systems.HistorySystems
{
    public class ApplyHistorySystem : IPreTickableExecutor
    {
        private readonly Descriptions _descriptions = new Descriptions();
        private readonly List<Entity> _entitiesCache = new List<Entity>();
        private readonly World _world;

        public ApplyHistorySystem(World world)
        {
            _world = world;
        }

        public void Tick(int tick, fix deltaTime)
        {
            HistoryIds.ForEachData(new ApplyHistoryAction(_descriptions, _entitiesCache, _world, tick));
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
            public readonly QueryDescription removeDesc = new QueryDescription().WithAll<History<T>, T>();
            public readonly QueryDescription addDesc = new QueryDescription().WithAll<History<T>>().WithNone<T>();
            public readonly QueryDescription tickDesc = new QueryDescription().WithAll<History<T>, T>();
        }

        private readonly struct ApplyHistoryAction : IStructGenericAction
        {
            private readonly Descriptions _descriptions;
            private readonly List<Entity> _entities;
            private readonly World _world;
            private readonly int _tick;

            public ApplyHistoryAction(Descriptions descriptions, List<Entity> entities, World world, int tick)
            {
                _descriptions = descriptions;
                _entities = entities;
                _world = world;
                _tick = tick;
            }

            public void Invoke<T>() where T : struct
            {
                var desc = _descriptions.GetDescription<T>();
                RemoveComponents<T>(desc.removeDesc);
                AddComponents<T>(desc.addDesc);
                SetComponentsData<T>(desc.tickDesc);
            }

            private void RemoveComponents<T>(QueryDescription desc) where T : struct
            {
                _entities.Clear();
                var query = new SelectToRemove<T>(_entities, _tick);
                _world.InlineEntityQuery<SelectToRemove<T>, History<T>>(desc, ref query);

                foreach (var item in _entities)
                    _world.Remove<T>(item);
            }

            private void AddComponents<T>(QueryDescription desc) where T : struct
            {
                _entities.Clear();
                var query = new SelectToAdd<T>(_entities, _tick);
                _world.InlineEntityQuery<SelectToAdd<T>, History<T>>(desc, ref query);

                foreach (var item in _entities)
                    _world.Add<T>(item);
            }

            private void SetComponentsData<T>(QueryDescription desc) where T : struct
            {
                var query = new SetHistoryQuery<T>(_tick);
                _world.InlineQuery<SetHistoryQuery<T>, History<T>, T>
                    (desc, ref query);
            }
        }

        private readonly struct SelectToAdd<T> : IForEachWithEntity<History<T>>
            where T : struct
        {
            private readonly List<Entity> _entities;
            private readonly int _tickIndex;

            public SelectToAdd(List<Entity> entities, int tick)
            {
                _entities = entities;
                _tickIndex = tick & Constants.HistoryTicksLimit - 1;
            }

            public readonly void Update(Entity entity, ref History<T> history)
            {
                if (history.history[_tickIndex].HasValue)
                    _entities.Add(entity);
            }
        }

        private readonly struct SelectToRemove<T> : IForEachWithEntity<History<T>>
            where T : struct
        {
            private readonly List<Entity> _entities;
            private readonly int _tickIndex;

            public SelectToRemove(List<Entity> entities, int tick)
            {
                _entities = entities;
                _tickIndex = tick & Constants.HistoryTicksLimit - 1;
            }

            public readonly void Update(Entity entity, ref History<T> history)
            {
                if (!history.history[_tickIndex].HasValue)
                    _entities.Add(entity);
            }
        }

        private readonly struct SetHistoryQuery<T> : IForEach<History<T>, T>
            where T : struct
        {
            private readonly int _tickIndex;

            public SetHistoryQuery(int tick)
            {
                _tickIndex = tick & Constants.HistoryTicksLimit - 1;
            }

            public readonly void Update(ref History<T> history, ref T component)
            {
                T? cmp = history.history[_tickIndex];
                Debug.Assert(cmp.HasValue);
                component = cmp.Value;
            }
        }
    }
}
