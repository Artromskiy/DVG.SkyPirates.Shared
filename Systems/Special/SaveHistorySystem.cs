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

        public SaveHistorySystem(World world)
        {
            _world = world;
            _componentIds = WorldComponentIds.For(world);
            _aliveIds = _componentIds.GetHistory(world.Layouts.GetPrimary(typeof(Alive)));
            _queries = Array.ConvertAll(_componentIds.History, component => new QuerySet(world, component, _aliveIds.Component));
        }

        public void Save(int tick)
        {
            var historyComponents = _componentIds.History;
            for (int i = 0; i < historyComponents.Length; i++)
            {
                AddHistory(historyComponents[i], _queries[i]);
            }

            for (int i = 0; i < historyComponents.Length; i++)
            {
                SaveComponentHistory(historyComponents[i], _queries[i], tick);
            }
        }

        public void SaveBaseline()
        {
            var historyComponents = _componentIds.History;
            for (int i = 0; i < historyComponents.Length; i++)
            {
                AddHistory(historyComponents[i], _queries[i]);
            }

            for (int i = 0; i < historyComponents.Length; i++)
            {
                var component = historyComponents[i];
                if (component.Component != _aliveIds.Component)
                {
                    SaveBaseline(component, _queries[i]);
                }
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
            var query = queries.Save;
            var context = new SaveContext(_world, component.History, currentTick);
            _world.ForEachEntity(in query, ref context, component.Component, typeof(SaveHistory<>));
        }

        private void SaveBaseline(HistoryComponentIds component, QuerySet queries)
        {
            var saveHas = queries.BaselineHas;
            int tick = int.MinValue;

            _world.ForEach(in saveHas, ref tick, component.Component, typeof(SaveBaselineHistory<>));
        }

        private static Query CreateAllQuery(World world, ComponentId first, ComponentId second)
        {
            Span<ComponentId> components = stackalloc ComponentId[2] { first, second };
            return world.WhereAll(components);
        }

        internal sealed class QuerySet
        {
            public readonly Query Save;
            public readonly Query MissingHistory;
            public readonly Query WithHistory;
            public readonly Query BaselineHas;

            public QuerySet(World world, HistoryComponentIds component, ComponentId aliveComponent)
            {
                Save = component.Component == aliveComponent
                    ? world.WhereAll(component.History)
                    : world.WhereAll(component.History).NotDisabled().Alive();
                MissingHistory = world.WhereAll(component.Component).WhereNone(component.History);
                WithHistory = CreateAllQuery(world, component.History, component.Component);
                BaselineHas = CreateAllQuery(world, component.History, component.Component).Alive();
            }
        }

        internal struct InitializeHistory<T> : IForEach where T : struct
        {
            public void Invoke(ref History<T> history)
            {
                if (history.Capacity == 0)
                {
                    history = new History<T>(4, Constants.MaxHistoryTicks);
                }
            }
        }

        internal struct SaveContext
        {
            public readonly World World;
            public readonly ComponentId History;
            public readonly int Tick;

            public SaveContext(World world, ComponentId history, int tick)
            {
                World = world;
                History = history;
                Tick = tick;
            }
        }

        internal struct SaveHistory<T> : IForEachContextEntity<SaveContext> where T : struct
        {
            public void Invoke(ref SaveContext context, Entity entity)
            {
                ref var history = ref context.World.GetRef<History<T>>(entity, context.History);
                history[context.Tick] = context.World.Has<T>(entity)
                    ? context.World.GetRef<T>(entity)
                    : null;
            }
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
