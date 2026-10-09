using System;
using System.Collections.Generic;
using Delta.ECS;
using DVG.Components;
using DVG.SkyPirates.Shared.Components.Framed;
using DVG.SkyPirates.Shared.Data;

namespace DVG.SkyPirates.Shared.Ecs
{
    /// <summary>Stores component history outside the ECS world.</summary>
    public sealed class WorldHistory : IDisposable
    {
        private readonly ComponentHistory[] _components;
        private readonly ComponentHistory<Alive> _aliveHistory;
        private readonly ComponentHistory<SyncId> _syncIdHistory;

        public WorldHistory(World world, ReadOnlySpan<ComponentId> componentIds, int initialCapacity, int maxCapacity)
        {
            if (world == null)
                throw new ArgumentNullException(nameof(world));
            if (initialCapacity <= 0 || (initialCapacity & (initialCapacity - 1)) != 0)
                throw new ArgumentOutOfRangeException(nameof(initialCapacity), "History capacity must be a positive power of two.");
            if (maxCapacity <= 0 || (maxCapacity & (maxCapacity - 1)) != 0 || initialCapacity > maxCapacity)
                throw new ArgumentOutOfRangeException(nameof(maxCapacity), "Maximum history capacity must be a power of two at least as large as the initial capacity.");

            var histories = new List<ComponentHistory>(componentIds.Length);
            var visitor = new CreateHistoryVisitor(world, histories, initialCapacity, maxCapacity);
            for (int i = 0; i < componentIds.Length; i++)
            {
                if (!world.Layouts.TryVisit(componentIds[i], visitor))
                {
                    Type componentType = world.Layouts.GetComponentType(componentIds[i]);
                    throw new InvalidOperationException(
                        $"Could not create history for registered component {componentType.FullName} (id {componentIds[i].Value}).");
                }
            }

            ComponentId aliveId = world.Layouts.GetPrimary<Alive>();
            ComponentId syncId = world.Layouts.GetPrimary<SyncId>();
            _aliveHistory = GetOrCreateHistory<Alive>(world, histories, aliveId, initialCapacity, maxCapacity);
            _syncIdHistory = GetOrCreateHistory<SyncId>(world, histories, syncId, initialCapacity, maxCapacity);
            _components = histories.ToArray();
        }

        public void Save(int tick)
        {
            for (int i = 0; i < _components.Length; i++)
                _components[i].Save(tick);
        }

        public void SaveBaseline()
        {
            for (int i = 0; i < _components.Length; i++)
                _components[i].SaveBaseline();
        }

        public void GoTo(int tick)
        {
            for (int i = 0; i < _components.Length; i++)
                _components[i].GoTo(tick);
        }

        public void Rollback(int tick)
        {
            GoTo(tick);
            for (int i = 0; i < _components.Length; i++)
                _components[i].Rollback(tick);
        }

        public WorldData GetSnapshot(int tick)
        {
            var snapshot = new WorldData();
            for (int i = 0; i < _components.Length; i++)
                _components[i].PackSnapshot(snapshot, tick, _aliveHistory, _syncIdHistory);

            return snapshot;
        }

        public void ApplySnapshot(WorldData snapshot, IReadOnlyDictionary<int, Entity> entitiesBySyncId)
        {
            for (int i = 0; i < _components.Length; i++)
                _components[i].ApplySnapshot(snapshot, entitiesBySyncId);
        }

        public void CollectExpiredEntities(int tick, List<Entity> entities)
        {
            _aliveHistory.CollectExpiredEntities(entities,
                static (value, lastTick, currentTick) => !value.HasValue && lastTick <= currentTick - Constants.MaxHistoryTicks,
                tick);
        }

        public void DisposeEntity(Entity entity)
        {
            for (int i = 0; i < _components.Length; i++)
                _components[i].DisposeEntity(entity);
        }

        public void Clear()
        {
            for (int i = 0; i < _components.Length; i++)
                _components[i].Clear();
        }

        public void Dispose()
        {
            for (int i = 0; i < _components.Length; i++)
                _components[i].Dispose();
        }

        private static ComponentHistory<T> GetOrCreateHistory<T>(
            World world,
            List<ComponentHistory> histories,
            ComponentId componentId,
            int initialCapacity,
            int maxCapacity) where T : struct
        {
            for (int i = 0; i < histories.Count; i++)
            {
                if (histories[i] is ComponentHistory<T> typed)
                    return typed;
            }

            var history = new ComponentHistory<T>(world, componentId, initialCapacity, maxCapacity);
            histories.Add(history);
            return history;
        }

        private sealed class CreateHistoryVisitor : IStructVisitor
        {
            private readonly World _world;
            private readonly List<ComponentHistory> _histories;
            private readonly int _initialCapacity;
            private readonly int _maxCapacity;

            public CreateHistoryVisitor(World world, List<ComponentHistory> histories, int initialCapacity, int maxCapacity)
            {
                _world = world;
                _histories = histories;
                _initialCapacity = initialCapacity;
                _maxCapacity = maxCapacity;
            }

            public void Visit<T>(ComponentId componentId) where T : struct
                => _histories.Add(new ComponentHistory<T>(_world, componentId, _initialCapacity, _maxCapacity));
        }

        private abstract class ComponentHistory
        {
            public abstract void Save(int tick);
            public abstract void SaveBaseline();
            public abstract void GoTo(int tick);
            public abstract void Rollback(int tick);
            public abstract void PackSnapshot(WorldData snapshot, int tick, ComponentHistory<Alive> alive, ComponentHistory<SyncId> syncId);
            public abstract void ApplySnapshot(WorldData snapshot, IReadOnlyDictionary<int, Entity> entitiesBySyncId);
            public abstract void DisposeEntity(Entity entity);
            public abstract void Clear();
            public abstract void Dispose();
        }

        private sealed class ComponentHistory<T> : ComponentHistory where T : struct
        {
            private readonly World _world;
            private readonly ComponentId _componentId;
            private readonly ComponentId _aliveId;
            private readonly ComponentId _disabledId;
            private readonly bool _isAlive;
            private readonly Query _saveQuery;
            private readonly Query _baselineQuery;
            private readonly ForEachContextEntityAction<CaptureState> _captureAction;
            private readonly IOperation<CaptureState> _saveOperation;
            private readonly IOperation<CaptureState> _baselineOperation;
            private readonly Dictionary<Entity, Entry> _entries = new();
            private readonly List<Entity> _staleEntities = new();

            public ComponentHistory(World world, ComponentId componentId, int initialCapacity, int maxCapacity)
            {
                _world = world;
                _componentId = componentId;
                _aliveId = world.Layouts.GetPrimary<Alive>();
                _disabledId = world.Layouts.GetPrimary<Disabled>();
                _isAlive = componentId == _aliveId;
                _saveQuery = CreateSaveQuery(world, componentId, _aliveId, _disabledId, _isAlive);
                _baselineQuery = CreateBaselineQuery(world, componentId, _aliveId);
                _captureAction = CaptureEntity;
                var initialState = default(CaptureState);
                _saveOperation = _world.ForEachEntity(in _saveQuery, ref initialState, _captureAction);
                _baselineOperation = _world.ForEachEntity(in _baselineQuery, ref initialState, _captureAction);
                InitialCapacity = initialCapacity;
                MaxCapacity = maxCapacity;
            }

            private int InitialCapacity { get; }
            private int MaxCapacity { get; }

            public override void Save(int tick)
            {
                foreach (var entry in _entries.Values)
                    entry.Seen = false;

                var context = new CaptureState(_world, this, _componentId, tick, false);
                _saveOperation.Invoke(ref context);

                _staleEntities.Clear();
                foreach (var pair in _entries)
                {
                    if (!_world.IsAlive(pair.Key))
                    {
                        _staleEntities.Add(pair.Key);
                        continue;
                    }

                    if (pair.Value.Seen || !IsTrackedForSave(pair.Key))
                        continue;

                    pair.Value.Values[tick] = null;
                }

                for (int i = 0; i < _staleEntities.Count; i++)
                    DisposeEntity(_staleEntities[i]);
            }

            public override void SaveBaseline()
            {
                foreach (var entry in _entries.Values)
                {
                    entry.Values.Rollback(int.MinValue);
                    entry.Values[int.MinValue] = null;
                    entry.LastSeenTick = int.MinValue;
                }

                var context = new CaptureState(_world, this, _componentId, int.MinValue, true);
                _baselineOperation.Invoke(ref context);
            }

            public override void GoTo(int tick)
            {
                foreach (var pair in _entries)
                {
                    Entity entity = pair.Key;
                    if (!_world.IsAlive(entity))
                        continue;

                    T? historicalValue = pair.Value.Values[tick];
                    if (!historicalValue.HasValue)
                    {
                        _world.Remove(entity, _componentId);
                        continue;
                    }

                    T value = historicalValue.Value;
                    if (!_world.TryGet<T>(entity, _componentId, out _))
                    {
                        _world.Add(entity, in value);
                    }
                    else
                    {
                        _world.GetRef<T>(entity, _componentId) = value;
                    }
                }
            }

            public override void Rollback(int tick)
            {
                foreach (var entry in _entries.Values)
                    entry.Values.Rollback(tick);
            }

            public bool TryGetAt(Entity entity, int tick, out T? value)
            {
                if (!_entries.TryGetValue(entity, out Entry entry) || entry.Values.Count == 0)
                {
                    value = null;
                    return false;
                }

                value = entry.Values[tick];
                return true;
            }

            public override void PackSnapshot(WorldData snapshot, int tick, ComponentHistory<Alive> alive, ComponentHistory<SyncId> syncId)
            {
                foreach (var pair in _entries)
                {
                    if (!TryGetAt(pair.Key, tick, out T? value) || !value.HasValue
                        || !alive.TryGetAt(pair.Key, tick, out Alive? aliveValue) || !aliveValue.HasValue
                        || !syncId.TryGetAt(pair.Key, tick, out SyncId? id) || !id.HasValue)
                    {
                        continue;
                    }

                    snapshot.Get<T>()[id.Value.Value] = value.Value;
                }
            }

            public override void ApplySnapshot(WorldData snapshot, IReadOnlyDictionary<int, Entity> entitiesBySyncId)
            {
                var components = snapshot.Get<T>();
                foreach (var pair in components)
                {
                    if (!entitiesBySyncId.TryGetValue(pair.Key, out Entity entity))
                        continue;

                    T value = pair.Value;
                    if (_world.TryGet<T>(entity, _componentId, out _))
                        _world.GetRef<T>(entity, _componentId) = value;
                    else
                        _world.Add(entity, in value);
                }
            }

            public void CollectExpiredEntities(List<Entity> entities, Func<T?, int, int, bool> shouldCollect, int currentTick)
            {
                foreach (var pair in _entries)
                {
                    T? last = pair.Value.Values.GetLast(out int lastTick);
                    if (shouldCollect(last, lastTick, currentTick))
                        entities.Add(pair.Key);
                }
            }

            public override void DisposeEntity(Entity entity)
            {
                if (_entries.TryGetValue(entity, out Entry entry))
                {
                    entry.Values.Dispose();
                    _entries.Remove(entity);
                }
            }

            public override void Clear()
            {
                foreach (var entry in _entries.Values)
                    entry.Values.Dispose();
                _entries.Clear();
            }

            public override void Dispose() => Clear();

            private Entry GetOrCreate(Entity entity)
            {
                if (!_entries.TryGetValue(entity, out Entry entry))
                {
                    entry = new Entry(InitialCapacity, MaxCapacity);
                    _entries.Add(entity, entry);
                }

                return entry;
            }

            private bool IsTrackedForSave(Entity entity)
            {
                if (_isAlive)
                    return _world.IsAlive(entity);

                return _world.Has(entity, _aliveId) && !_world.Has(entity, _disabledId);
            }

            internal void Record(Entity entity, T value, int tick, bool baseline)
            {
                Entry entry = GetOrCreate(entity);
                entry.Seen = true;
                if (baseline)
                {
                    entry.Values[int.MinValue] = value;
                    entry.LastSeenTick = int.MinValue;
                }
                else
                {
                    entry.Values[tick] = value;
                    entry.LastSeenTick = tick;
                }
            }

            private static Query CreateSaveQuery(World world, ComponentId componentId, ComponentId aliveId, ComponentId disabledId, bool isAlive)
            {
                if (isAlive)
                    return world.WhereAll(componentId);

                Span<ComponentId> required = stackalloc ComponentId[2] { componentId, aliveId };
                return world.WhereAll(required).WhereNone(disabledId);
            }

            private static Query CreateBaselineQuery(World world, ComponentId componentId, ComponentId aliveId)
            {
                if (componentId == aliveId)
                    return world.WhereAll(componentId);

                Span<ComponentId> required = stackalloc ComponentId[2] { componentId, aliveId };
                return world.WhereAll(required);
            }

            private static void CaptureEntity(ref CaptureState state, EntityRef entity)
            {
                if (state.World.TryGet<T>(entity.Handle, state.ComponentId, out T value))
                {
                    state.Store.Record(entity.Handle, value, state.Tick, state.Baseline);
                }
            }

            private struct CaptureState
            {
                public readonly World World;
                public readonly ComponentHistory<T> Store;
                public readonly ComponentId ComponentId;
                public readonly int Tick;
                public readonly bool Baseline;
                public CaptureState(World world, ComponentHistory<T> store, ComponentId componentId, int tick, bool baseline)
                {
                    World = world;
                    Store = store;
                    ComponentId = componentId;
                    Tick = tick;
                    Baseline = baseline;
                }
            }

            private sealed class Entry
            {
                public History<T> Values;
                public int LastSeenTick;
                public bool Seen;

                public Entry(int initialCapacity, int maxCapacity)
                {
                    Values = new History<T>(initialCapacity, maxCapacity);
                }
            }
        }

    }
}
