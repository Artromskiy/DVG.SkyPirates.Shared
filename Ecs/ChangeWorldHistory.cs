using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Delta.ECS;
using DVG.Components;
using DVG.SkyPirates.Shared.Data;

namespace DVG.SkyPirates.Shared.Ecs
{
    /// <summary>Stores sparse component changes reported by DeltaECS instead of full component frames.</summary>
    public sealed class ChangeWorldHistory : IWorldHistory
    {
        private const int NotRetired = int.MaxValue;

        private readonly World _world;
        private readonly ComponentId[] _componentIds;
        private readonly ComponentId _syncIdId;
        private readonly int _initialCapacity;
        private readonly int _maxCapacity;
        private readonly Dictionary<Entity, RetirementHistory> _retirements = new();
        private ComponentHistory[] _components = Array.Empty<ComponentHistory>();
        private ComponentHistory<SyncId> _syncIdHistory = null!;

        public ChangeWorldHistory(World world, ReadOnlySpan<ComponentId> componentIds, int initialCapacity, int maxCapacity)
        {
            if (world == null)
            {
                throw new ArgumentNullException(nameof(world));
            }

            ValidateCapacity(initialCapacity, nameof(initialCapacity));
            ValidateCapacity(maxCapacity, nameof(maxCapacity));
            if (initialCapacity > maxCapacity)
            {
                throw new ArgumentOutOfRangeException(nameof(maxCapacity), "Maximum history capacity must be at least the initial capacity.");
            }

            _world = world;
            _syncIdId = world.Components.GetPrimary<SyncId>();
            _initialCapacity = initialCapacity;
            _maxCapacity = maxCapacity;
            _componentIds = CopyComponentIds(componentIds, _syncIdId);
            ResetComponentHistories();
        }

        public void Save(int tick) => Capture(tick, false);

        public void SaveBaseline()
        {
            ResetComponentHistories();
            foreach (RetirementHistory retirement in _retirements.Values)
            {
                retirement.Values.Rollback(int.MinValue);
                retirement.Values[int.MinValue] = retirement.Current;
            }

            Capture(int.MinValue, true);
        }

        public void GoTo(int tick)
        {
            foreach (RetirementHistory retirement in _retirements.Values)
            {
                retirement.Restore(retirement.Values[tick]);
            }

            for (int i = 0; i < _components.Length; i++)
            {
                _components[i].GoTo(tick);
            }
        }

        public void Rollback(int tick)
        {
            GoTo(tick);
            foreach (RetirementHistory retirement in _retirements.Values)
            {
                retirement.Values.Rollback(tick);
            }

            for (int i = 0; i < _components.Length; i++)
            {
                _components[i].Rollback(tick);
            }
        }

        public WorldData GetSnapshot(int tick)
        {
            var snapshot = new WorldData();
            Dictionary<Entity, int> syncIds = _syncIdHistory.GetSyncIdsAt(tick);
            for (int i = 0; i < _components.Length; i++)
            {
                _components[i].PackSnapshot(snapshot, tick, syncIds);
            }

            return snapshot;
        }

        public void ApplySnapshot(WorldData snapshot, IReadOnlyDictionary<int, Entity> entitiesBySyncId)
        {
            foreach (Entity entity in entitiesBySyncId.Values)
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

            RetirementHistory retirement = GetOrCreateRetirement(entity);
            retirement.RetireFromSnapshot();
        }

        public void MarkForDisposal(Entity entity, int tick)
        {
            RetirementHistory retirement = GetOrCreateRetirement(entity);
            retirement.MarkForDisposal(tick);
        }

        public void CollectExpiredEntities(int tick, List<Entity> entities)
        {
            foreach (var pair in _retirements)
            {
                if (pair.Value.IsExpired(tick))
                {
                    entities.Add(pair.Key);
                }
            }
        }

        public void DisposeEntity(Entity entity)
        {
            RemoveRetirement(entity);
            for (int i = 0; i < _components.Length; i++)
            {
                _components[i].Forget(entity);
            }
        }

        public void Clear()
        {
            foreach (RetirementHistory retirement in _retirements.Values)
            {
                retirement.Dispose();
            }

            _retirements.Clear();
            ResetComponentHistories();
        }

        public void Dispose()
        {
            foreach (RetirementHistory retirement in _retirements.Values)
            {
                retirement.Dispose();
            }

            _retirements.Clear();
            for (int i = 0; i < _components.Length; i++)
            {
                _components[i].Dispose();
            }

            _components = Array.Empty<ComponentHistory>();
        }

        private void Capture(int tick, bool baseline)
        {
            for (int i = 0; i < _components.Length; i++)
            {
                _components[i].BeginCapture(tick);
            }

            for (int i = 0; i < _components.Length; i++)
            {
                _components[i].CaptureChanges();
            }

            CompleteRetirements(tick, baseline);
        }

        private void CompleteRetirements(int tick, bool baseline)
        {
            List<Entity> stale = null;
            foreach (var pair in _retirements)
            {
                RetirementHistory retirement = pair.Value;
                if (!_world.Has(pair.Key, _syncIdId))
                {
                    (stale ??= new List<Entity>()).Add(pair.Key);
                    continue;
                }

                if (!baseline && retirement.RetirementPending)
                {
                    retirement.CompleteRetirement(tick);
                }

                retirement.Record(tick);
            }

            if (stale == null)
            {
                return;
            }

            for (int i = 0; i < stale.Count; i++)
            {
                RemoveRetirement(stale[i]);
            }
        }

        private void ResetComponentHistories()
        {
            for (int i = 0; i < _components.Length; i++)
            {
                _components[i].Dispose();
            }

            var histories = new List<ComponentHistory>(_componentIds.Length);
            var visitor = new CreateHistoryVisitor(this, histories);
            for (int i = 0; i < _componentIds.Length; i++)
            {
                _world.Components.Visit(_componentIds[i], visitor);
            }

            _components = histories.ToArray();
            for (int i = 0; i < _components.Length; i++)
            {
                if (_components[i].ComponentId == _syncIdId)
                {
                    _syncIdHistory = (ComponentHistory<SyncId>)_components[i];
                    return;
                }
            }
        }

        private RetirementHistory GetOrCreateRetirement(Entity entity)
        {
            if (_retirements.TryGetValue(entity, out RetirementHistory retirement))
            {
                return retirement;
            }

            retirement = new RetirementHistory(_initialCapacity, _maxCapacity);
            _retirements.Add(entity, retirement);
            return retirement;
        }

        private void RemoveRetirement(Entity entity)
        {
            if (_retirements.TryGetValue(entity, out RetirementHistory retirement))
            {
                retirement.Dispose();
                _retirements.Remove(entity);
            }
        }

        private static ComponentId[] CopyComponentIds(ReadOnlySpan<ComponentId> componentIds, ComponentId syncIdId)
        {
            var result = new List<ComponentId>(componentIds.Length + 1);
            bool hasSyncId = false;
            for (int i = 0; i < componentIds.Length; i++)
            {
                ComponentId componentId = componentIds[i];
                bool duplicate = false;
                for (int j = 0; j < result.Count; j++)
                {
                    if (result[j] == componentId)
                    {
                        duplicate = true;
                        break;
                    }
                }

                if (!duplicate)
                {
                    result.Add(componentId);
                }

                hasSyncId |= componentId == syncIdId;
            }

            if (!hasSyncId)
            {
                result.Add(syncIdId);
            }

            return result.ToArray();
        }

        private static void ValidateCapacity(int capacity, string name)
        {
            if (capacity <= 0 || (capacity & (capacity - 1)) != 0)
            {
                throw new ArgumentOutOfRangeException(name, "History capacity must be a positive power of two.");
            }
        }

        private sealed class CreateHistoryVisitor : IStructVisitor
        {
            private readonly ChangeWorldHistory _owner;
            private readonly List<ComponentHistory> _histories;

            public CreateHistoryVisitor(ChangeWorldHistory owner, List<ComponentHistory> histories)
            {
                _owner = owner;
                _histories = histories;
            }

            public void Visit<T>(ComponentId componentId) where T : struct
                => _histories.Add(new ComponentHistory<T>(_owner, componentId, _owner._initialCapacity, _owner._maxCapacity));
        }

        private abstract class ComponentHistory : IDisposable
        {
            protected ComponentHistory(ComponentId componentId)
            {
                ComponentId = componentId;
            }

            public ComponentId ComponentId { get; }
            public abstract void BeginCapture(int tick);
            public abstract void CaptureChanges();
            public abstract void GoTo(int tick);
            public abstract void Rollback(int tick);
            public abstract void PackSnapshot(WorldData snapshot, int tick, IReadOnlyDictionary<Entity, int> syncIds);
            public abstract void ApplySnapshot(WorldData snapshot, IReadOnlyDictionary<int, Entity> entitiesBySyncId);
            public abstract void RemoveFrom(Entity entity);
            public abstract void Forget(Entity entity);
            public abstract void Dispose();
        }

        private sealed class ComponentHistory<T> : ComponentHistory where T : struct
        {
            private readonly ChangeWorldHistory _owner;
            private readonly ComponentId _componentId;
            private readonly ComponentChangeOperation<T> _operation;
            private readonly ChangeFrameHistory<T> _frames;
            private readonly HashSet<Entity> _knownEntities = new();
            private ChangeFrame<T> _captureFrame = null!;

            public ComponentHistory(ChangeWorldHistory owner, ComponentId componentId, int initialCapacity, int maxCapacity)
                : base(componentId)
            {
                _owner = owner;
                _componentId = componentId;
                _frames = new ChangeFrameHistory<T>(initialCapacity, maxCapacity);
                _operation = owner._world.ForEachChange<T>(componentId, OnChange);
            }

            public override void BeginCapture(int tick) => _captureFrame = _frames.BeginWrite(tick);

            public override void CaptureChanges() => _operation.Invoke();

            public override void GoTo(int tick)
            {
                Dictionary<Entity, T> state = _frames.GetState(tick);
                foreach (Entity entity in _knownEntities)
                {
                    if (!_owner._world.IsAlive(entity))
                    {
                        continue;
                    }

                    bool shouldExist = state.TryGetValue(entity, out T value);
                    bool exists = _owner._world.Has(entity, _componentId);
                    if (shouldExist)
                    {
                        if (exists)
                        {
                            _owner._world.GetRef<T>(entity, _componentId) = value;
                        }
                        else
                        {
                            _owner._world.Add(entity, in value);
                        }
                    }
                    else if (exists)
                    {
                        _owner._world.Remove(entity, _componentId);
                    }
                }
            }

            public override void Rollback(int tick) => _frames.Rollback(tick);

            public override void PackSnapshot(WorldData snapshot, int tick, IReadOnlyDictionary<Entity, int> syncIds)
            {
                Dictionary<Entity, T> state = _frames.GetState(tick);
                Dictionary<int, T> output = snapshot.Get<T>();
                foreach (var pair in state)
                {
                    if (syncIds.TryGetValue(pair.Key, out int syncId))
                    {
                        output[syncId] = pair.Value;
                    }
                }
            }

            public override void ApplySnapshot(WorldData snapshot, IReadOnlyDictionary<int, Entity> entitiesBySyncId)
            {
                Dictionary<int, T> components = snapshot.Get<T>();
                foreach (var pair in components)
                {
                    if (!entitiesBySyncId.TryGetValue(pair.Key, out Entity entity))
                    {
                        continue;
                    }

                    T value = pair.Value;
                    if (_owner._world.Has(entity, _componentId))
                    {
                        _owner._world.GetRef<T>(entity, _componentId) = value;
                    }
                    else
                    {
                        _owner._world.Add(entity, in value);
                    }
                }
            }

            public override void RemoveFrom(Entity entity)
            {
                if (_owner._world.Has(entity, _componentId))
                {
                    _owner._world.Remove(entity, _componentId);
                }
            }

            public override void Forget(Entity entity)
            {
                _knownEntities.Remove(entity);
                _frames.Forget(entity);
            }

            public override void Dispose() => _frames.Dispose();

            private void OnChange(Entity entity, ChangeKind change, ref T component)
            {
                if (change == ChangeKind.Removed)
                {
                    _captureFrame.Add(entity, change, default);
                    if (_componentId == _owner._syncIdId)
                    {
                        _owner.RemoveRetirement(entity);
                    }

                    return;
                }

                _knownEntities.Add(entity);
                _captureFrame.Add(entity, change, in component);
            }

            public Dictionary<Entity, int> GetSyncIdsAt(int tick)
            {
                Dictionary<Entity, T> state = _frames.GetState(tick);
                var result = new Dictionary<Entity, int>(state.Count);
                foreach (var pair in state)
                {
                    T value = pair.Value;
                    SyncId syncId = Unsafe.As<T, SyncId>(ref value);
                    result.Add(pair.Key, syncId.Value);
                }

                return result;
            }
        }

        private sealed class ChangeFrameHistory<T> : IDisposable where T : struct
        {
            private ChangeFrame<T>[] _frames;
            private readonly Dictionary<Entity, T> _baseline = new();
            private int _mask;
            private int _head;
            private int _count;
            private readonly int _maxCapacity;

            public ChangeFrameHistory(int initialCapacity, int maxCapacity)
            {
                _frames = new ChangeFrame<T>[initialCapacity];
                _mask = initialCapacity - 1;
                _maxCapacity = maxCapacity;
                _frames[0] = new ChangeFrame<T>(int.MinValue);
                _count = 1;
            }

            public ChangeFrame<T> BeginWrite(int tick)
            {
                if (tick < Last.Tick)
                {
                    Rollback(tick);
                }

                if (Last.Tick == tick)
                {
                    return Last;
                }

                EnsureCapacity();
                int index = (_head + _count) & _mask;
                ChangeFrame<T> frame = _frames[index] ?? (_frames[index] = new ChangeFrame<T>(tick));
                frame.Reset(tick);
                _count++;
                return frame;
            }

            public Dictionary<Entity, T> GetState(int tick)
            {
                if (tick < First.Tick)
                {
                    throw new IndexOutOfRangeException($"History does not contain tick {tick}");
                }

                var state = new Dictionary<Entity, T>(_baseline);
                for (int i = 0; i < _count; i++)
                {
                    ChangeFrame<T> frame = At(i);
                    if (frame.Tick > tick)
                    {
                        break;
                    }

                    frame.ApplyTo(state);
                }

                return state;
            }

            public void Rollback(int tick)
            {
                if (_count == 0 || tick < First.Tick)
                {
                    throw new IndexOutOfRangeException($"History does not contain tick {tick}");
                }

                while (_count > 1 && Last.Tick > tick)
                {
                    Last.Clear();
                    _count--;
                }
            }

            public void Forget(Entity entity)
            {
                _baseline.Remove(entity);
                for (int i = 0; i < _count; i++)
                {
                    At(i).Forget(entity);
                }
            }

            public void Dispose()
            {
                for (int i = 0; i < _frames.Length; i++)
                {
                    _frames[i]?.Clear();
                    _frames[i] = null;
                }

                _baseline.Clear();
                _count = 0;
            }

            private void EnsureCapacity()
            {
                if (_count < _frames.Length)
                {
                    return;
                }

                if (_frames.Length < _maxCapacity)
                {
                    int capacity = Math.Min(_frames.Length << 1, _maxCapacity);
                    var expanded = new ChangeFrame<T>[capacity];
                    for (int i = 0; i < _count; i++)
                    {
                        expanded[i] = At(i);
                    }

                    _frames = expanded;
                    _head = 0;
                    _mask = capacity - 1;
                    return;
                }

                ChangeFrame<T> expired = First;
                expired.ApplyTo(_baseline);
                expired.Clear();
                _head = (_head + 1) & _mask;
                _count--;
            }

            private ChangeFrame<T> First => At(0);
            private ChangeFrame<T> Last => At(_count - 1);
            private ChangeFrame<T> At(int logicalIndex) => _frames[(_head + logicalIndex) & _mask];
        }

        private sealed class ChangeFrame<T> where T : struct
        {
            private readonly List<Change<T>> _changes = new();

            public int Tick { get; private set; }

            public ChangeFrame(int tick)
            {
                Tick = tick;
            }

            public void Reset(int tick)
            {
                _changes.Clear();
                Tick = tick;
            }

            public void Add(Entity entity, ChangeKind kind, in T value)
                => _changes.Add(new Change<T>(entity, kind, in value));

            public void ApplyTo(Dictionary<Entity, T> state)
            {
                for (int i = 0; i < _changes.Count; i++)
                {
                    Change<T> change = _changes[i];
                    if (change.Kind == ChangeKind.Removed)
                    {
                        state.Remove(change.Entity);
                    }
                    else
                    {
                        state[change.Entity] = change.Value;
                    }
                }
            }

            public void Forget(Entity entity)
            {
                for (int i = _changes.Count - 1; i >= 0; i--)
                {
                    if (_changes[i].Entity == entity)
                    {
                        _changes.RemoveAt(i);
                    }
                }
            }

            public void Clear() => _changes.Clear();
        }

        private readonly struct Change<T> where T : struct
        {
            public readonly Entity Entity;
            public readonly ChangeKind Kind;
            public readonly T Value;

            public Change(Entity entity, ChangeKind kind, in T value)
            {
                Entity = entity;
                Kind = kind;
                Value = value;
            }
        }

        private sealed class RetirementHistory : IDisposable
        {
            public History<RetirementState> Values;
            public int RetiredAt = NotRetired;
            public bool RetirementPending;

            public RetirementHistory(int initialCapacity, int maxCapacity)
            {
                Values = new History<RetirementState>(initialCapacity, maxCapacity);
            }

            public RetirementState Current => new(RetiredAt, RetirementPending);

            public void RetireFromSnapshot()
            {
                RetiredAt = NotRetired;
                RetirementPending = true;
            }

            public void MarkForDisposal(int tick)
            {
                RetiredAt = tick;
                RetirementPending = false;
                Values[tick] = Current;
            }

            public void CompleteRetirement(int tick)
            {
                RetiredAt = tick;
                RetirementPending = false;
            }

            public void Record(int tick) => Values[tick] = Current;

            public void Restore(RetirementState? state)
            {
                RetiredAt = state.HasValue ? state.Value.RetiredAt : NotRetired;
                RetirementPending = state.HasValue && state.Value.RetirementPending;
            }

            public bool IsExpired(int tick)
            {
                RetirementState? state = Values.GetLast(out _);
                return state.HasValue
                    && !state.Value.RetirementPending
                    && state.Value.RetiredAt != NotRetired
                    && state.Value.RetiredAt <= tick - Constants.MaxHistoryTicks;
            }

            public void Dispose() => Values.Dispose();
        }

        private readonly struct RetirementState
        {
            public readonly int RetiredAt;
            public readonly bool RetirementPending;

            public RetirementState(int retiredAt, bool retirementPending)
            {
                RetiredAt = retiredAt;
                RetirementPending = retirementPending;
            }
        }
    }
}
