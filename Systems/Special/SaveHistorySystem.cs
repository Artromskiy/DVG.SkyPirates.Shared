using System;
using Delta;
using Delta.ECS;
using DVG.Components;
using DVG.SkyPirates.Shared.Ecs;

namespace DVG.SkyPirates.Shared.Systems.Special
{
    internal sealed class SaveHistorySystem
    {
        private readonly World _world;
        private readonly WorldComponentIds _componentIds;
        private readonly HistoryComponentIds _aliveIds;
        private readonly QuerySet[] _queries;

        private Query? _hasAliveCache;
        private Query _hasAlive => _hasAliveCache ??= CreateHasAliveQuery();
        private Query? _noAliveCache;
        private Query _noAlive => _noAliveCache ??= CreateNoAliveQuery();

        public SaveHistorySystem(World world)
        {
            _world = world;
            _componentIds = WorldComponentIds.For(world);
            _aliveIds = _componentIds.GetHistory(world.Layouts.GetPrimary(typeof(Alive)));
            _queries = Array.ConvertAll(_componentIds.History, component => new QuerySet(world, component));
        }

        public void Save(int tick)
        {
            var historyComponents = _componentIds.History;
            for (var i = 0; i < historyComponents.Length; i++)
                AddHistory(historyComponents[i], _queries[i]);

            for (var i = 0; i < historyComponents.Length; i++)
            {
                var component = historyComponents[i];
                if (component.Component != _aliveIds.Component)
                    SaveComponentHistory(component, _queries[i], tick);
            }

            var hasAlive = _hasAlive;
            var noAlive = _noAlive;
            var aliveId = _aliveIds.Component;
            _world.ForEach(in hasAlive, ref tick, aliveId, typeof(SaveHistory<>));
            _world.ForEach(in noAlive, ref tick, aliveId, typeof(SaveMissingHistory<>));
        }

        public void SaveBaseline()
        {
            var historyComponents = _componentIds.History;
            for (var i = 0; i < historyComponents.Length; i++)
                AddHistory(historyComponents[i], _queries[i]);

            for (var i = 0; i < historyComponents.Length; i++)
            {
                var component = historyComponents[i];
                if (component.Component != _aliveIds.Component)
                    SaveBaseline(component, _queries[i]);
            }
        }

        private void AddHistory(HistoryComponentIds component, QuerySet queries)
        {
            var missingHistory = queries.MissingHistory;
            Span<ComponentId> historyComponent = stackalloc ComponentId[1] { component.History };
            _world.Add(in missingHistory, historyComponent);

            var withHistory = queries.WithHistory;
            _world.ForEach(in withHistory, component.Component, typeof(InitializeHistory<>));
        }

        private void SaveComponentHistory(HistoryComponentIds component, QuerySet queries, int currentTick)
        {
            var saveHas = queries.SaveHas;
            var saveMissing = queries.SaveMissing;
            var tick = currentTick;

            _world.ForEach(in saveHas, ref tick, component.Component, typeof(SaveHistory<>));
            _world.ForEach(in saveMissing, ref tick, component.Component, typeof(SaveMissingHistory<>));
        }

        private void SaveBaseline(HistoryComponentIds component, QuerySet queries)
        {
            var saveHas = queries.BaselineHas;
            var tick = int.MinValue;

            _world.ForEach(in saveHas, ref tick, component.Component, typeof(SaveBaselineHistory<>));
        }

        private Query CreateHasAliveQuery()
        {
            Span<ComponentId> components = stackalloc ComponentId[2] { _aliveIds.History, _aliveIds.Component };
            return _world.WhereAll(components);
        }

        private Query CreateNoAliveQuery()
        {
            return _world.WhereAll(_aliveIds.History).WhereNone(_aliveIds.Component);
        }

        private static Query CreateAllQuery(World world, ComponentId first, ComponentId second)
        {
            Span<ComponentId> components = stackalloc ComponentId[2] { first, second };
            return world.WhereAll(components);
        }

        internal sealed class QuerySet
        {
            public readonly Query MissingHistory;
            public readonly Query WithHistory;
            public readonly Query SaveHas;
            public readonly Query SaveMissing;
            public readonly Query BaselineHas;

            public QuerySet(World world, HistoryComponentIds component)
            {
                MissingHistory = world.WhereAll(component.Component).WhereNone(component.History);
                WithHistory = CreateAllQuery(world, component.History, component.Component);
                SaveHas = CreateAllQuery(world, component.History, component.Component).NotDisabled().Alive();
                SaveMissing = world.WhereAll(component.History).WhereNone(component.Component).NotDisabled().Alive();
                BaselineHas = CreateAllQuery(world, component.History, component.Component).Alive();
            }
        }

        internal struct InitializeHistory<T> : IForEach where T : struct
        {
            public void Invoke(ref History<T> history)
            {
                if (history.Capacity == 0)
                    history = new History<T>(4, Constants.MaxHistoryTicks);
            }
        }

        internal struct SaveHistory<T> : IForEachContext<int> where T : struct
        {
            public void Invoke(ref int tick, ref History<T> history, in T component)
                => history[tick] = component;
        }

        internal struct SaveMissingHistory<T> : IForEachContext<int> where T : struct
        {
            public void Invoke(ref int tick, ref History<T> history)
                => history[tick] = null;
        }

        internal struct SaveBaselineHistory<T> : IForEachContext<int> where T : struct
        {
            public void Invoke(ref int tick, ref History<T> history, in T component)
            {
                history.Rollback(tick);
                history[tick] = component;
            }
        }

    }
}
