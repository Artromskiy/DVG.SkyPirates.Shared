using Arch.Core;
using DVG.Collections;
using DVG.Components;
using System;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Systems.Special
{
    internal sealed class RollbackHistorySystem
    {
        private sealed class Description<T> where T : struct
        {
            public readonly QueryDescription removeDesc = new QueryDescription().WithAll<History<T>, T>();
            public readonly QueryDescription addDesc = new QueryDescription().WithAll<History<T>>().WithNone<T>();
            public readonly QueryDescription tickDesc = new QueryDescription().WithAll<History<T>, T>();

            public readonly QueryDescription historyDesc = new QueryDescription().WithAll<History<T>>();
        }

        private readonly QueryDescription _noSyncId = new QueryDescription().WithNone<SyncId>();

        private readonly GenericCreator _creator = new();
        private readonly List<Entity> _entitiesCache = new();
        private readonly World _world;

        public RollbackHistorySystem(World world)
        {
            _world = world;
        }


        public void GoTo(int tick)
        {
            var action = new SetHistoryAction(_creator, _entitiesCache, _world, tick);
            HistoryComponentsRegistry.ForEachData(ref action);
        }

        public void RollBack(int tick)
        {
            var action = new SetHistoryAction(_creator, _entitiesCache, _world, tick);
            HistoryComponentsRegistry.ForEachData(ref action);
            var clearAction = new ClearHistoryAction(_world, tick, _creator);
            HistoryComponentsRegistry.ForEachData(ref clearAction);
            _world.Destroy(_noSyncId);
        }

        private readonly struct ClearHistoryAction : IStructGenericAction
        {
            private readonly World _world;
            private readonly int _tick;
            private readonly GenericCreator _creator;

            public ClearHistoryAction(World world, int tick, GenericCreator creator)
            {
                _world = world;
                _tick = tick;
                _creator = creator;
            }

            public void Invoke<T>() where T : struct
            {
                ClearHistoryQuery<T> clearQuery = new(_tick);
                var desc = _creator.Get<Description<T>>().historyDesc;
                _world.InlineQuery<ClearHistoryQuery<T>, History<T>>(in desc, ref clearQuery);
            }

            private readonly struct ClearHistoryQuery<T> : IForEach<History<T>> where T : struct
            {
                private readonly int _tick;
                public ClearHistoryQuery(int tick) => _tick = tick;

                public readonly void Update(ref History<T> history) =>
                    history.Rollback(_tick);
            }
        }

        private readonly struct SetHistoryAction : IStructGenericAction
        {
            private readonly GenericCreator _creator;
            private readonly List<Entity> _entities;
            private readonly World _world;
            private readonly int _tick;

            public SetHistoryAction(GenericCreator creator, List<Entity> entities, World world, int tick)
            {
                _creator = creator;
                _entities = entities;
                _world = world;
                _tick = tick;
            }

            public void Invoke<T>() where T : struct
            {
                var desc = _creator.Get<Description<T>>();
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
            private readonly int _tick;

            public SelectToAdd(List<Entity> entities, int tick)
            {
                _entities = entities;
                _tick = tick;
            }

            public readonly void Update(Entity entity, ref History<T> history)
            {
                if (history[_tick].HasValue)
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
                if (!history[_tick].HasValue)
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
                var cmp = history[_tick];

                if (!cmp.HasValue)
                    throw new InvalidOperationException();

                component = cmp.Value;
            }
        }
    }
}
