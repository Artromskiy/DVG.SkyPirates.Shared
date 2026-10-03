using System;
using System.Collections.Generic;
using Delta.ECS;
using DVG.Collections;
using DVG.Components;
using DVG.SkyPirates.Shared.Ecs;

namespace DVG.SkyPirates.Shared.Systems.Special
{
    internal sealed class RollbackHistorySystem
    {
        private readonly List<Entity> _entitiesCache = new();
        private readonly ComponentQueryCache<QuerySet> _queryCache = new();
        private readonly World _world;
        private readonly WorldComponentIds _componentIds;

        internal struct ClearHistory<T> : IForEachContext<int> where T : struct
        {
            public void Invoke(ref int tick, ref History<T> history)
                => history.Rollback(tick);
        }

        internal struct SelectComponentsToRemove<T> : IForEachContextEntity<EntitySelectionState> where T : struct
        {
            public void Invoke(ref EntitySelectionState selection, Entity entity, ref History<T> history)
            {
                if (history.Count == 0 || !history[selection.Tick].HasValue)
                    selection.Entities.Add(entity);
            }
        }

        internal struct SelectComponentsToAdd<T> : IForEachContextEntity<EntitySelectionState> where T : struct
        {
            public void Invoke(ref EntitySelectionState selection, Entity entity, ref History<T> history)
            {
                if (history.Count > 0 && history[selection.Tick].HasValue)
                    selection.Entities.Add(entity);
            }
        }

        internal struct ApplyHistory<T> : IForEachContext<int> where T : struct
        {
            public void Invoke(ref int tick, ref History<T> history, ref T component)
            {
                var value = history[tick];
                if (!value.HasValue)
                    throw new InvalidOperationException();

                component = value.Value;
            }
        }

        public RollbackHistorySystem(World world)
        {
            _world = world;
            _componentIds = WorldComponentIds.For(world);
        }

        public void GoTo(int tick)
        {
            var histories = _componentIds.History;
            for (var i = 0; i < histories.Length; i++)
                SetHistory(histories[i], tick);
        }

        // TODO can optimize to destroy first and apply then
        public void RollBack(int tick)
        {
            var histories = _componentIds.History;
            for (var i = 0; i < histories.Length; i++)
                SetHistory(histories[i], tick);

            for (var i = 0; i < histories.Length; i++)
                ClearHistories(histories[i], tick);
        }

        private void ClearHistories(HistoryComponentIds component, int targetTick)
        {
            var queries = _queryCache.Get(component.Component);
            var filter = queries.Clear ??= CreateHistoryQuery(component);
            var tick = targetTick;
            _world.ForEach(in filter, ref tick, component.Component, typeof(ClearHistory<>));
        }

        private void SetHistory(HistoryComponentIds component, int targetTick)
        {
            var queries = _queryCache.Get(component.Component);
            var withComponent = queries.WithComponent ??= CreateWithComponentQuery(component);
            var withoutComponent = queries.WithoutComponent ??= CreateWithoutComponentQuery(component);

            RemoveComponents(withComponent, component, targetTick);
            AddComponents(withoutComponent, component, targetTick);

            var tick = targetTick;
            _world.ForEach(in withComponent, ref tick, component.Component, typeof(ApplyHistory<>));
        }

        private void RemoveComponents(Query filter, HistoryComponentIds component, int targetTick)
        {
            _entitiesCache.Clear();
            var state = new EntitySelectionState(_entitiesCache, targetTick);
            _world.ForEachEntity(in filter, ref state, component.Component, typeof(SelectComponentsToRemove<>));

            Span<ComponentId> componentIds = stackalloc ComponentId[1] { component.Component };
            foreach (var entity in _entitiesCache)
                _world.Remove(entity, componentIds);
        }

        private void AddComponents(Query filter, HistoryComponentIds component, int targetTick)
        {
            _entitiesCache.Clear();
            var state = new EntitySelectionState(_entitiesCache, targetTick);
            _world.ForEachEntity(in filter, ref state, component.Component, typeof(SelectComponentsToAdd<>));

            Span<ComponentId> componentIds = stackalloc ComponentId[1] { component.Component };
            foreach (var entity in _entitiesCache)
                _world.Add(entity, componentIds);
        }

        private Query CreateHistoryQuery(HistoryComponentIds component)
        {
            return _world.WhereAll(component.History);
        }

        private Query CreateWithComponentQuery(HistoryComponentIds component)
        {
            Span<ComponentId> all = stackalloc ComponentId[2] { component.History, component.Component };
            return _world.WhereAll(all);
        }

        private Query CreateWithoutComponentQuery(HistoryComponentIds component)
        {
            return _world.WhereAll(component.History).WhereNone(component.Component);
        }

        internal sealed class QuerySet
        {
            public QuerySet() { }

            public Query? Clear;
            public Query? WithComponent;
            public Query? WithoutComponent;
        }

        internal struct EntitySelectionState
        {
            public readonly List<Entity> Entities;
            public readonly int Tick;

            public EntitySelectionState(List<Entity> entities, int tick)
            {
                Entities = entities;
                Tick = tick;
            }
        }
    }
}
