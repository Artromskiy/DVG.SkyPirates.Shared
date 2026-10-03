using Delta.ECS;
using DVG.SkyPirates.Shared.Ecs;
using DVG.Components;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;
using System;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Systems.Special
{
    internal class DisposeSystem : IDisposeSystem
    {
        private readonly ComponentQueryCache<QuerySet> _queryCache = new();
        private Query? _disposingDescCache;
        private Query _disposingDesc => _disposingDescCache ??= _world.WhereAll<History<Alive>>().WhereNone<Alive>();
        private Query? _toDestroyCache;
        private Query _toDestroy => _toDestroyCache ??= _world.WhereAll<Temp>();
        private readonly List<Entity> _entitiesCache = new();
        private readonly World _world;
        private readonly WorldComponentIds _componentIds;
        private readonly ComponentId _tempComponentId;

        internal struct DisposeHistory<T> : IForEach where T : struct
        {
            public void Invoke(ref History<T> history) => history.Dispose();
        }

        public DisposeSystem(World world)
        {
            _world = world;
            _componentIds = WorldComponentIds.For(world);
            _tempComponentId = world.Layouts.GetPrimary<Temp>();
        }

        public void Tick(int tick)
        {
            _entitiesCache.Clear();
            (List<Entity> Entities, int CurrentTick) state = (_entitiesCache, tick);
            var disposingDesc = _disposingDesc;
            _world.ForEachEntity<(List<Entity> Entities, int CurrentTick), History<Alive>>(in disposingDesc, ref state,
                static (ref (List<Entity> Entities, int CurrentTick) state, Entity entity, ref History<Alive> aliveHistory) =>
                {
                    if (aliveHistory.GetLast(out int tick) == null && tick <= state.CurrentTick - Constants.MaxHistoryTicks)
                    {
                        state.Entities.Add(entity);
                    }
                });
            foreach (var entity in _entitiesCache)
            {
                _world.Add<Temp>(entity);
            }

            var disposableComponents = _componentIds.Disposable;
            for (int i = 0; i < disposableComponents.Length; i++)
            {
                DisposeComponents(disposableComponents[i]);
            }

            var historyComponents = _componentIds.History;
            for (int i = 0; i < historyComponents.Length; i++)
            {
                DisposeHistoryComponents(historyComponents[i]);
            }

            var toDestroy = _toDestroy;
            _world.Destroy(in toDestroy);
        }

        private void DisposeComponents(DisposableComponentIds component)
        {
            var queries = _queryCache.Get(component.Component);
            var filter = queries.Components ??= CreateComponentQuery(component.Component);
            component.Dispose(in filter);
        }

        private void DisposeHistoryComponents(HistoryComponentIds component)
        {
            var queries = _queryCache.Get(component.Component);
            var filter = queries.History ??= CreateHistoryQuery(component.History);
            _world.ForEach(in filter, component.Component, typeof(DisposeHistory<>));
        }

        private Query CreateComponentQuery(ComponentId componentId)
        {
            Span<ComponentId> all = stackalloc ComponentId[2] { componentId, _tempComponentId };
            return _world.WhereAll(all);
        }

        private Query CreateHistoryQuery(ComponentId historyComponentId)
        {
            Span<ComponentId> all = stackalloc ComponentId[2] { historyComponentId, _tempComponentId };
            return _world.WhereAll(all);
        }

        internal sealed class QuerySet
        {
            public QuerySet() { }

            public Query? Components;
            public Query? History;
        }
    }
}
