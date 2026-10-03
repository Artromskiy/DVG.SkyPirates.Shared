using System;
using System.Collections.Generic;
using Delta.ECS;
using DVG.Components;
using DVG.SkyPirates.Shared.Data;
using DVG.SkyPirates.Shared.Ecs;
using DVG.SkyPirates.Shared.IFactories;
using DVG.SkyPirates.Shared.IServices;

namespace DVG.SkyPirates.Shared.Systems.Special
{
    public class SnapshotHistorySystem
    {
        private readonly List<Entity> _entitiesCache = new();
        private readonly List<Entity> _snapshotEntitiesCache = new();
        private readonly World _world;
        private readonly WorldComponentIds _componentIds;
        private readonly HistoryComponentIds _aliveIds;
        private readonly HistoryComponentIds _syncIdIds;
        private readonly Query[] _packQueries;
        private readonly IEntityFactory _entityFactory;
        private readonly IEntityRegistry _entityRegistry;

        public SnapshotHistorySystem(World world, IEntityFactory entityFactory, IEntityRegistry entityRegistry)
        {
            _world = world;
            _componentIds = WorldComponentIds.For(world);
            _aliveIds = _componentIds.GetHistory(world.Layouts.GetPrimary(typeof(Alive)));
            _syncIdIds = _componentIds.GetHistory(world.Layouts.GetPrimary(typeof(SyncId)));
            var historyComponents = _componentIds.History;
            _packQueries = new Query[historyComponents.Length];
            Span<ComponentId> identityComponents = stackalloc ComponentId[2] { _aliveIds.History, _syncIdIds.History };
            var identityQuery = _world.WhereAll(identityComponents);
            for (var i = 0; i < historyComponents.Length; i++)
            {
                var component = historyComponents[i];
                _packQueries[i] = component.Component == _aliveIds.Component || component.Component == _syncIdIds.Component
                    ? identityQuery
                    : CreateComponentPackQuery(component);
            }
            _entityFactory = entityFactory;
            _entityRegistry = entityRegistry;
        }

        public WorldData GetSnapshot(int tick)
        {
            var worldData = new WorldData();
            var context = new PackState { Components = worldData, Tick = tick };
            var historyComponents = _componentIds.History;

            for (var i = 0; i < historyComponents.Length; i++)
            {
                var component = historyComponents[i];
                var query = _packQueries[i];
                var functor = component.Component == _aliveIds.Component
                    ? typeof(PackAliveHistory<>)
                    : component.Component == _syncIdIds.Component
                        ? typeof(PackSyncIdHistory<>)
                        : typeof(PackComponentHistory<>);
                _world.ForEach(in query, ref context, component.Component, functor);
            }

            return worldData;
        }

        public void ApplySnapshot(WorldData snapshot)
        {
            foreach (var syncId in snapshot.Get<SyncId>().Values)
                _entityRegistry.Reserve(syncId);

            foreach (var syncIdReserve in snapshot.Get<SyncIdReserve>().Values)
                _entityRegistry.Reserve(syncIdReserve);

            _entitiesCache.Clear();
            _snapshotEntitiesCache.Clear();
            var alive = snapshot.Get<Alive>();
            Span<ComponentId> aliveComponent = stackalloc ComponentId[1] { _aliveIds.Component };
            foreach (var syncId in snapshot.Get<SyncId>().Values)
            {
                var entity = _entityFactory.Create(new()
                {
                    SyncId = syncId,
                    RandomSeed = default,
                    SyncIdReserve = default,
                });

                _snapshotEntitiesCache.Add(entity);
                if (!alive.ContainsKey(syncId.Value))
                    _world.Remove(entity, aliveComponent);
            }

            var snapshotEntities = _snapshotEntitiesCache.ToArray();
            var historyComponents = _componentIds.History;
            for (var i = 0; i < historyComponents.Length; i++)
                ApplyComponentSnapshot(snapshot, historyComponents[i], snapshotEntities);

        }

        private void ApplyComponentSnapshot(WorldData snapshot, HistoryComponentIds component, Entity[] snapshotEntities)
        {
            _entitiesCache.Clear();
            var state = new ApplySnapshotState(_world, component.Component, snapshot, _entitiesCache);
            _world.ForEachEntity(snapshotEntities, ref state, component.Component, typeof(SelectSnapshotEntities<>));

            if (_entitiesCache.Count == 0)
                return;

            Entity[] entities = _entitiesCache.ToArray();
            _world.Add(entities, component.Component);
            _world.ForEachEntity(entities, ref state, component.Component, typeof(ApplySnapshotComponent<>));
        }

        private Query CreateComponentPackQuery(HistoryComponentIds component)
        {
            Span<ComponentId> all = stackalloc ComponentId[3]
            {
                component.History,
                _syncIdIds.History,
                _aliveIds.History,
            };
            return _world.WhereAll(all);
        }

        internal struct PackState
        {
            public WorldData Components;
            public int Tick;
        }

        internal struct PackAliveHistory<T> : IForEachContext<PackState> where T : struct
        {
            public void Invoke(ref PackState state, ref History<Alive> alive, ref History<SyncId> syncId)
            {
                var aliveComponent = alive[state.Tick];
                if (!aliveComponent.HasValue)
                    return;

                var idComponent = syncId[state.Tick];
                if (idComponent.HasValue)
                    state.Components.Get<T>()[idComponent.Value.Value] = (T)(object)aliveComponent.Value;
            }
        }

        internal struct PackSyncIdHistory<T> : IForEachContext<PackState> where T : struct
        {
            public void Invoke(ref PackState state, ref History<SyncId> syncId, ref History<Alive> alive)
            {
                var aliveComponent = alive[state.Tick];
                if (!aliveComponent.HasValue)
                    return;

                var idComponent = syncId[state.Tick];
                if (idComponent.HasValue)
                    state.Components.Get<T>()[idComponent.Value.Value] = (T)(object)idComponent.Value;
            }
        }

        internal struct PackComponentHistory<T> : IForEachContext<PackState> where T : struct
        {
            public void Invoke(ref PackState state, ref History<T> history, ref History<SyncId> id, ref History<Alive> alive)
            {
                var historyComponent = history[state.Tick];
                if (!historyComponent.HasValue || !alive[state.Tick].HasValue)
                    return;

                var idComponent = id[state.Tick];
                if (idComponent.HasValue)
                    state.Components.Get<T>()[idComponent.Value.Value] = historyComponent.Value;
            }
        }

        internal struct ApplySnapshotState
        {
            public readonly World World;
            public readonly ComponentId Component;
            public readonly WorldData Snapshot;
            public readonly List<Entity> Entities;

            public ApplySnapshotState(World world, ComponentId component, WorldData snapshot, List<Entity> entities)
            {
                World = world;
                Component = component;
                Snapshot = snapshot;
                Entities = entities;
            }
        }

        internal struct SelectSnapshotEntities<T> : IForEachContextEntity<ApplySnapshotState> where T : struct
        {
            public void Invoke(ref ApplySnapshotState state, Entity entity)
            {
                if (state.Snapshot.Get<T>().ContainsKey(state.World.GetRef<SyncId>(entity).Value))
                    state.Entities.Add(entity);
            }
        }

        internal struct ApplySnapshotComponent<T> : IForEachContextEntity<ApplySnapshotState> where T : struct
        {
            public void Invoke(ref ApplySnapshotState state, Entity entity)
            {
                var syncId = state.World.GetRef<SyncId>(entity).Value;
                state.World.GetRef<T>(entity, state.Component) = state.Snapshot.Get<T>()[syncId];
            }
        }

    }
}
