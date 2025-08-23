using Arch.Core;
using DVG.Core;
using DVG.Core.History;
using DVG.SkyPirates.Shared.Archetypes;
using DVG.SkyPirates.Shared.Components.Special;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;
using System.Collections.Generic;
using System.Diagnostics;

namespace DVG.SkyPirates.Shared.Services.TickableExecutors.HistorySystems
{
    public class ApplyHistorySystem : IPreTickableExecutor
    {
        private readonly List<Entity> _entitiesCache = new List<Entity>();
        private readonly World _world;

        public ApplyHistorySystem(World world)
        {
            _world = world;
        }

        public void Tick(int tick, fix deltaTime)
        {
            HistoryIds.ForEachData(new ApplyHistoryAction(_entitiesCache, _world, tick));
        }

        private readonly struct ApplyHistoryAction : IStructGenericAction
        {
            private readonly List<Entity> _entities;
            private readonly World _world;
            private readonly int _tick;

            public ApplyHistoryAction(List<Entity> entities, World world, int tick)
            {
                _entities = entities;
                _world = world;
                _tick = tick;
            }

            public void Invoke<T>() where T : struct
            {
                RemoveComponents<T>();
                AddComponents<T>();
                SetComponentsData<T>();
            }

            private void RemoveComponents<T>() where T : struct
            {
                _entities.Clear();
                var query = new SelectToRemove<T>(_entities, _tick);
                _world.InlineEntityQuery<SelectToRemove<T>, History<T>>(
                    new QueryDescription().WithAll<History<T>, T>(), ref query);

                foreach (var item in _entities)
                    _world.Remove<T>(item);
            }

            private void AddComponents<T>() where T : struct
            {
                _entities.Clear();
                var query = new SelectToAdd<T>(_entities, _tick);
                _world.InlineEntityQuery<SelectToAdd<T>, History<T>>(
                    new QueryDescription().WithAll<History<T>>().WithNone<T>(), ref query);

                foreach (var item in _entities)
                    _world.Add<T>(item);
            }

            private void SetComponentsData<T>() where T : struct
            {
                var query = new SetHistoryQuery<T>(_tick);
                _world.InlineQuery<SetHistoryQuery<T>, History<T>, T>
                    (new QueryDescription().WithAll<History<T>, T>(), ref query);
            }
        }

        private readonly struct SelectToAdd<T> : IForEachWithEntity<History<T>>
            where T : struct
        {
            private readonly List<Entity> _entities;
            private readonly int _tick;

            public SelectToAdd(List<Entity> entities, int tick)
            {
                _entities = entities;
                _tick = tick;
            }

            public readonly void Update(Entity entity, ref History<T> history)
            {
                if (history.history.TryGetValue(_tick, out T? cmp) && cmp.HasValue)
                    _entities.Add(entity);
            }
        }

        private readonly struct SelectToRemove<T> : IForEachWithEntity<History<T>>
            where T : struct
        {
            private readonly List<Entity> _entities;
            private readonly int _tick;

            public SelectToRemove(List<Entity> entities, int tick)
            {
                _entities = entities;
                _tick = tick;
            }

            public readonly void Update(Entity entity, ref History<T> history)
            {
                if (!history.history.TryGetValue(_tick, out T? cmp) || !cmp.HasValue)
                    _entities.Add(entity);
            }
        }

        private readonly struct SetHistoryQuery<T> : IForEach<History<T>, T>
            where T : struct
        {
            private readonly int _tick;

            public SetHistoryQuery(int tick)
            {
                _tick = tick;
            }

            public readonly void Update(ref History<T> history, ref T component)
            {
                T? cmp = history.history[_tick];
                Debug.Assert(cmp.HasValue);
                component = cmp.Value;
            }
        }
    }
}
