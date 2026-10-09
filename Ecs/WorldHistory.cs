using System;
using System.Collections.Generic;
using Delta.ECS;
using DVG.Components;
using DVG.SkyPirates.Shared.Data;

namespace DVG.SkyPirates.Shared.Ecs
{
    /// <summary>Stores component history outside the ECS world.</summary>
    public sealed class WorldHistory : IDisposable
    {
        private readonly ComponentHistory[] _components;
        private readonly ComponentHistory<SyncId> _syncIdHistory;
        private readonly EntityPresenceHistory _entityPresence;

        public WorldHistory(World world, ReadOnlySpan<ComponentId> componentIds, int initialCapacity, int maxCapacity)
        {
            if (world == null)
            {
                throw new ArgumentNullException(nameof(world));
            }

            if (initialCapacity <= 0 || (initialCapacity & (initialCapacity - 1)) != 0)
            {
                throw new ArgumentOutOfRangeException(nameof(initialCapacity), "History capacity must be a positive power of two.");
            }

            if (maxCapacity <= 0 || (maxCapacity & (maxCapacity - 1)) != 0 || initialCapacity > maxCapacity)
            {
                throw new ArgumentOutOfRangeException(nameof(maxCapacity), "Maximum history capacity must be a power of two at least as large as the initial capacity.");
            }

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

            ComponentId syncId = world.Layouts.GetPrimary<SyncId>();
            _syncIdHistory = GetOrCreateHistory<SyncId>(world, histories, syncId, initialCapacity, maxCapacity);
            _entityPresence = new EntityPresenceHistory(world, syncId, initialCapacity, maxCapacity);
            _components = histories.ToArray();
        }

        public void Save(int tick)
        {
            _entityPresence.Save(tick);
            for (int i = 0; i < _components.Length; i++)
            {
                _components[i].Save(tick);
            }
        }

        public void SaveBaseline()
        {
            _entityPresence.SaveBaseline();
            for (int i = 0; i < _components.Length; i++)
            {
                _components[i].SaveBaseline();
            }
        }

        public void GoTo(int tick)
        {
            _entityPresence.GoTo(tick);
            for (int i = 0; i < _components.Length; i++)
            {
                _components[i].GoTo(tick);
            }
        }

        public void Rollback(int tick)
        {
            GoTo(tick);
            _entityPresence.Rollback(tick);
            for (int i = 0; i < _components.Length; i++)
            {
                _components[i].Rollback(tick);
            }
        }

        public WorldData GetSnapshot(int tick)
        {
            var snapshot = new WorldData();
            for (int i = 0; i < _components.Length; i++)
            {
                _components[i].PackSnapshot(snapshot, tick, _entityPresence, _syncIdHistory);
            }

            return snapshot;
        }

        public void ApplySnapshot(WorldData snapshot, IReadOnlyDictionary<int, Entity> entitiesBySyncId)
        {
            foreach (var entity in entitiesBySyncId.Values)
            {
                for (int i = 0; i < _components.Length; i++)
                {
                    _components[i].RemoveFrom(entity);
                }
            }

            for (int i = 0; i < _components.Length; i++)
            {
                _components[i].ApplySnapshot(snapshot, entitiesBySyncId);
            }
        }

        public void RetireEntityFromSnapshot(Entity entity)
        {
            for (int i = 0; i < _components.Length; i++)
            {
                _components[i].RemoveFrom(entity);
            }

            _entityPresence.RetireFromSnapshot(entity);
        }

        // Keeps retired entities in snapshots until their rollback window expires.
        public void MarkForDisposal(Entity entity, int tick) => _entityPresence.MarkForDisposal(entity, tick);

        public void CollectExpiredEntities(int tick, List<Entity> entities) =>
            _entityPresence.CollectExpiredEntities(tick, entities);

        public void DisposeEntity(Entity entity)
        {
            _entityPresence.DisposeEntity(entity);
            for (int i = 0; i < _components.Length; i++)
            {
                _components[i].DisposeEntity(entity);
            }
        }

        public void Clear()
        {
            _entityPresence.Clear();
            for (int i = 0; i < _components.Length; i++)
            {
                _components[i].Clear();
            }
        }

        public void Dispose()
        {
            _entityPresence.Dispose();
            for (int i = 0; i < _components.Length; i++)
            {
                _components[i].Dispose();
            }
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
                {
                    return typed;
                }
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

        private sealed class EntityPresenceHistory : IDisposable
        {
            private const int NotRetired = int.MaxValue;

            private readonly World _world;
            private readonly IOperation<CaptureState> _saveOperation;
            private readonly Dictionary<Entity, Entry> _entries = new();
            private readonly List<Entity> _staleEntities = new();
            private readonly int _initialCapacity;
            private readonly int _maxCapacity;

            public EntityPresenceHistory(World world, ComponentId syncId, int initialCapacity, int maxCapacity)
            {
                _world = world;
                var query = world.WhereAll(syncId);
                _initialCapacity = initialCapacity;
                _maxCapacity = maxCapacity;
                ForEachContextEntityAction<CaptureState> captureAction = CaptureEntity;
                var state = default(CaptureState);
                _saveOperation = world.ForEachEntity(in query, ref state, captureAction);
            }

            public void Save(int tick) => Capture(tick, false);

            public void SaveBaseline()
            {
                foreach (var entry in _entries.Values)
                {
                    entry.Values.Rollback(int.MinValue);
                    entry.Values[int.MinValue] = entry.RetirementPending
                        ? new EntityState(false, NotRetired, true)
                        : null;
                }

                Capture(int.MinValue, true);
            }

            public bool ExistsAt(Entity entity, int tick) =>
                _entries.TryGetValue(entity, out Entry entry)
                && entry.Values[tick] is EntityState state
                && state.Exists;

            public void MarkForDisposal(Entity entity, int tick)
            {
                Entry entry = GetOrCreate(entity);
                entry.RetiredAt = tick;
                entry.RetirementPending = false;
            }

            public void RetireFromSnapshot(Entity entity)
            {
                Entry entry = GetOrCreate(entity);
                entry.RetiredAt = NotRetired;
                entry.RetirementPending = true;
            }

            public void CollectExpiredEntities(int tick, List<Entity> entities)
            {
                foreach (var pair in _entries)
                {
                    EntityState? state = pair.Value.Values.GetLast(out _);
                    if (state.HasValue && !state.Value.RetirementPending
                                       && state.Value.RetiredAt != NotRetired
                                       && state.Value.RetiredAt <= tick - Constants.MaxHistoryTicks)
                    {
                        entities.Add(pair.Key);
                    }
                }
            }

            public void Rollback(int tick)
            {
                foreach (var entry in _entries.Values)
                {
                    entry.Values.Rollback(tick);
                    RestoreCurrentState(entry, entry.Values.GetLast(out _));
                }
            }

            public void GoTo(int tick)
            {
                foreach (var entry in _entries.Values)
                {
                    RestoreCurrentState(entry, entry.Values[tick]);
                }
            }

            public void DisposeEntity(Entity entity)
            {
                if (_entries.TryGetValue(entity, out Entry entry))
                {
                    entry.Values.Dispose();
                    _entries.Remove(entity);
                }
            }

            public void Clear()
            {
                foreach (var entry in _entries.Values)
                {
                    entry.Values.Dispose();
                }

                _entries.Clear();
            }

            public void Dispose() => Clear();

            private void Capture(int tick, bool baseline)
            {
                foreach (var entry in _entries.Values)
                {
                    entry.Seen = false;
                }

                var state = new CaptureState(this, tick, baseline);
                _saveOperation.Invoke(ref state);

                _staleEntities.Clear();
                foreach (var pair in _entries)
                {
                    Entry entry = pair.Value;
                    if (!_world.IsAlive(pair.Key))
                    {
                        _staleEntities.Add(pair.Key);
                        continue;
                    }

                    if (entry.Seen)
                    {
                        continue;
                    }

                    if (!baseline && entry.RetirementPending)
                    {
                        entry.RetiredAt = tick;
                        entry.RetirementPending = false;
                    }
                    else if (!entry.RetirementPending && entry.RetiredAt == NotRetired)
                    {
                        if (baseline)
                        {
                            entry.RetirementPending = true;
                        }
                        else
                        {
                            entry.RetiredAt = tick;
                        }
                    }

                    entry.Values[tick] = new EntityState(false, entry.RetiredAt, entry.RetirementPending);
                }

                for (int i = 0; i < _staleEntities.Count; i++)
                {
                    DisposeEntity(_staleEntities[i]);
                }
            }

            private Entry GetOrCreate(Entity entity)
            {
                if (!_entries.TryGetValue(entity, out Entry entry))
                {
                    entry = new Entry(_initialCapacity, _maxCapacity);
                    _entries.Add(entity, entry);
                }

                return entry;
            }

            private static void RestoreCurrentState(Entry entry, EntityState? state)
            {
                entry.RetiredAt = state?.RetiredAt ?? NotRetired;
                entry.RetirementPending = state.HasValue && state.Value.RetirementPending;
            }

            private static void CaptureEntity(ref CaptureState state, EntityRef entity)
            {
                Entry entry = state.History.GetOrCreate(entity.Handle);
                entry.Seen = true;

                if (!state.Baseline && entry.RetirementPending)
                {
                    entry.RetiredAt = state.Tick;
                    entry.RetirementPending = false;
                }

                entry.Values[state.Tick] = new EntityState(true, entry.RetiredAt, entry.RetirementPending);
            }

            private struct CaptureState
            {
                public readonly EntityPresenceHistory History;
                public readonly int Tick;
                public readonly bool Baseline;

                public CaptureState(EntityPresenceHistory history, int tick, bool baseline)
                {
                    History = history;
                    Tick = tick;
                    Baseline = baseline;
                }
            }

            private sealed class Entry
            {
                public History<EntityState> Values;
                public bool Seen;
                public int RetiredAt = NotRetired;
                public bool RetirementPending;

                public Entry(int initialCapacity, int maxCapacity)
                {
                    Values = new History<EntityState>(initialCapacity, maxCapacity);
                }
            }

            private readonly struct EntityState
            {
                public readonly bool Exists;
                public readonly int RetiredAt;
                public readonly bool RetirementPending;

                public EntityState(bool exists, int retiredAt, bool retirementPending)
                {
                    Exists = exists;
                    RetiredAt = retiredAt;
                    RetirementPending = retirementPending;
                }
            }
        }

        private abstract class ComponentHistory
        {
            public abstract void Save(int tick);
            public abstract void SaveBaseline();
            public abstract void GoTo(int tick);
            public abstract void Rollback(int tick);
            public abstract void PackSnapshot(WorldData snapshot, int tick, EntityPresenceHistory entityPresence, ComponentHistory<SyncId> syncId);
            public abstract void ApplySnapshot(WorldData snapshot, IReadOnlyDictionary<int, Entity> entitiesBySyncId);
            public abstract void RemoveFrom(Entity entity);
            public abstract void DisposeEntity(Entity entity);
            public abstract void Clear();
            public abstract void Dispose();
        }

        internal struct MissingCaptureState
        {
            public readonly Action<Entity, int> RecordMissing;
            public readonly int Tick;

            public MissingCaptureState(Action<Entity, int> recordMissing, int tick)
            {
                RecordMissing = recordMissing;
                Tick = tick;
            }
        }

        internal delegate void ComponentCaptureAction<T>(Entity entity, in T value, int tick, bool baseline) where T : struct;

        internal struct ComponentCaptureState<T> where T : struct
        {
            public readonly ComponentCaptureAction<T> Capture;
            public readonly int Tick;
            public readonly bool Baseline;

            public ComponentCaptureState(ComponentCaptureAction<T> capture, int tick, bool baseline)
            {
                Capture = capture;
                Tick = tick;
                Baseline = baseline;
            }
        }

        internal struct CaptureComponent<T> : IForEachContextEntity<ComponentCaptureState<T>> where T : struct
        {
            public void Invoke(ref ComponentCaptureState<T> state, EntityRef entity, in T component)
                => state.Capture(entity.Handle, in component, state.Tick, state.Baseline);
        }

        private sealed class ComponentHistory<T> : ComponentHistory where T : struct
        {
            private readonly World _world;
            private readonly ComponentId _componentId;
            private readonly Query _missingQuery;
            private readonly IOperation<ComponentCaptureState<T>> _saveOperation;
            private readonly ComponentCaptureAction<T> _captureComponent;
            private readonly Action<Entity, int> _recordMissing;
            private Entry?[] _entriesByEntityIndex = Array.Empty<Entry?>();
            private readonly List<int> _trackedEntityIndices = new();
            private readonly List<Entity> _staleEntities = new();
            private Entity[] _trackedEntities = Array.Empty<Entity>();
            private int _trackedEntityCount;

            public ComponentHistory(World world, ComponentId componentId, int initialCapacity, int maxCapacity)
            {
                _world = world;
                _componentId = componentId;
                var saveQuery = CreateComponentQuery(world, componentId);
                _missingQuery = world.WhereNone(componentId);
                _captureComponent = CaptureComponent;
                _recordMissing = RecordMissing;
                var initialState = default(ComponentCaptureState<T>);
                _saveOperation = _world.ForEachEntity(
                    in saveQuery,
                    ref initialState,
                    _componentId,
                    typeof(CaptureComponent<>));
                InitialCapacity = initialCapacity;
                MaxCapacity = maxCapacity;
            }

            private int InitialCapacity { get; }
            private int MaxCapacity { get; }

            public override void Save(int tick)
            {
                PrepareTrackedEntities();

                var context = new ComponentCaptureState<T>(_captureComponent, tick, false);
                _saveOperation.Invoke(ref context);
                CaptureMissingEntities(tick);

                _staleEntities.Clear();
                for (int i = 0; i < _trackedEntityIndices.Count; i++)
                {
                    Entry entry = _entriesByEntityIndex[_trackedEntityIndices[i]]!;
                    if (!_world.IsAlive(entry.Entity))
                    {
                        _staleEntities.Add(entry.Entity);
                    }
                }

                for (int i = 0; i < _staleEntities.Count; i++)
                {
                    DisposeEntity(_staleEntities[i]);
                }
            }

            public override void SaveBaseline()
            {
                for (int i = 0; i < _trackedEntityIndices.Count; i++)
                {
                    Entry entry = _entriesByEntityIndex[_trackedEntityIndices[i]]!;
                    entry.Values.Rollback(int.MinValue);
                    entry.Values[int.MinValue] = null;
                }

                var context = new ComponentCaptureState<T>(_captureComponent, int.MinValue, true);
                PrepareTrackedEntities();
                _saveOperation.Invoke(ref context);
                CaptureMissingEntities(int.MinValue);
            }

            public override void GoTo(int tick)
            {
                for (int i = 0; i < _trackedEntityIndices.Count; i++)
                {
                    Entry entry = _entriesByEntityIndex[_trackedEntityIndices[i]]!;
                    Entity entity = entry.Entity;
                    if (!_world.IsAlive(entity))
                    {
                        continue;
                    }

                    T? historicalValue = entry.Values[tick];
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
                for (int i = 0; i < _trackedEntityIndices.Count; i++)
                {
                    _entriesByEntityIndex[_trackedEntityIndices[i]]!.Values.Rollback(tick);
                }
            }

            private bool TryGetAt(Entity entity, int tick, out T? value)
            {
                if (!TryGetEntry(entity, out Entry entry) || entry.Values.Count == 0)
                {
                    value = null;
                    return false;
                }

                value = entry.Values[tick];
                return true;
            }

            public override void PackSnapshot(WorldData snapshot, int tick, EntityPresenceHistory entityPresence, ComponentHistory<SyncId> syncId)
            {
                for (int i = 0; i < _trackedEntityIndices.Count; i++)
                {
                    Entity entity = _entriesByEntityIndex[_trackedEntityIndices[i]]!.Entity;
                    if (!TryGetAt(entity, tick, out T? value) || !value.HasValue
                                                              || !entityPresence.ExistsAt(entity, tick)
                                                              || !syncId.TryGetAt(entity, tick, out SyncId? id) || !id.HasValue)
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
                    {
                        continue;
                    }

                    T value = pair.Value;
                    if (_world.TryGet<T>(entity, _componentId, out _))
                    {
                        _world.GetRef<T>(entity, _componentId) = value;
                    }
                    else
                    {
                        _world.Add(entity, in value);
                    }
                }
            }

            public override void RemoveFrom(Entity entity)
            {
                if (_world.IsAlive(entity) && _world.Has(entity, _componentId))
                {
                    _world.Remove(entity, _componentId);
                }
            }

            public override void DisposeEntity(Entity entity)
            {
                if (TryGetEntry(entity, out Entry entry))
                {
                    RemoveEntry(entity.Index, entry);
                }
            }

            public override void Clear()
            {
                for (int i = 0; i < _trackedEntityIndices.Count; i++)
                {
                    int entityIndex = _trackedEntityIndices[i];
                    _entriesByEntityIndex[entityIndex]!.Values.Dispose();
                    _entriesByEntityIndex[entityIndex] = null;
                }

                _trackedEntityIndices.Clear();
            }

            public override void Dispose() => Clear();

            private Entry GetOrCreate(Entity entity)
            {
                EnsureEntityIndexCapacity(entity.Index);
                Entry? entry = _entriesByEntityIndex[entity.Index];
                if (entry != null && entry.Entity == entity)
                {
                    return entry;
                }

                if (entry != null)
                {
                    RemoveEntry(entity.Index, entry);
                }

                entry = new Entry(entity, InitialCapacity, MaxCapacity, _trackedEntityIndices.Count);
                _entriesByEntityIndex[entity.Index] = entry;
                _trackedEntityIndices.Add(entity.Index);
                return entry;
            }

            private bool TryGetEntry(Entity entity, out Entry entry)
            {
                int entityIndex = entity.Index;
                var candidate = _entriesByEntityIndex[entityIndex];
                if ((uint)entityIndex < (uint)_entriesByEntityIndex.Length
                    && candidate != null && candidate.Entity == entity)
                {
                    entry = candidate;
                    return true;
                }

                entry = null!;
                return false;
            }

            private void RemoveEntry(int entityIndex, Entry entry)
            {
                entry.Values.Dispose();
                int trackedIndex = entry.TrackedIndex;
                int lastTrackedIndex = _trackedEntityIndices.Count - 1;
                int lastEntityIndex = _trackedEntityIndices[lastTrackedIndex];
                _trackedEntityIndices[trackedIndex] = lastEntityIndex;
                _entriesByEntityIndex[lastEntityIndex]!.TrackedIndex = trackedIndex;
                _trackedEntityIndices.RemoveAt(lastTrackedIndex);
                _entriesByEntityIndex[entityIndex] = null;
            }

            private void EnsureEntityIndexCapacity(int entityIndex)
            {
                if (entityIndex < _entriesByEntityIndex.Length)
                {
                    return;
                }

                int capacity = _entriesByEntityIndex.Length == 0 ? 4 : _entriesByEntityIndex.Length;
                while (capacity <= entityIndex)
                {
                    capacity = checked(capacity * 2);
                }

                Array.Resize(ref _entriesByEntityIndex, capacity);
            }

            private void Record(Entity entity, T value, int tick, bool baseline)
            {
                Entry entry = GetOrCreate(entity);
                if (baseline)
                {
                    entry.Values[int.MinValue] = value;
                }
                else
                {
                    entry.Values[tick] = value;
                }
            }

            private static Query CreateComponentQuery(World world, ComponentId componentId)
                => world.WhereAll(componentId);

            private void PrepareTrackedEntities()
            {
                _trackedEntityCount = 0;
                if (_trackedEntities.Length < _trackedEntityIndices.Count)
                {
                    Array.Resize(ref _trackedEntities, Math.Max(_trackedEntityIndices.Count, _trackedEntities.Length * 2));
                }

                for (int i = 0; i < _trackedEntityIndices.Count; i++)
                {
                    _trackedEntities[_trackedEntityCount++] = _entriesByEntityIndex[_trackedEntityIndices[i]]!.Entity;
                }
            }

            private void CaptureMissingEntities(int tick)
            {
                var context = new MissingCaptureState(_recordMissing, tick);
                _world.ForEachEntity(
                        _trackedEntities.AsSpan(0, _trackedEntityCount),
                        in _missingQuery,
                        ref context,
                        static (ref MissingCaptureState state, EntityRef entity) =>
                            state.RecordMissing(entity.Handle, state.Tick))
                    .Invoke(ref context);
            }

            private void CaptureComponent(Entity entity, in T component, int tick, bool baseline)
                => Record(entity, component, tick, baseline);

            private void RecordMissing(Entity entity, int tick)
            {
                if (TryGetEntry(entity, out Entry entry))
                {
                    entry.Values[tick] = null;
                }
            }

            private sealed class Entry
            {
                public readonly Entity Entity;
                public History<T> Values;
                public int TrackedIndex;

                public Entry(Entity entity, int initialCapacity, int maxCapacity, int trackedIndex)
                {
                    Entity = entity;
                    Values = new History<T>(initialCapacity, maxCapacity);
                    TrackedIndex = trackedIndex;
                }
            }
        }
    }
}
