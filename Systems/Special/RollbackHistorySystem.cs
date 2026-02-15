using Arch.Core;
using DVG.Collections;
using DVG.Components;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;
using System;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Systems.Special
{
    internal sealed class RollbackHistorySystem : IRollbackHistorySystem
    {
        private sealed class Description<T> where T : struct
        {
            public readonly QueryDescription removeDesc = new QueryDescription().WithAll<History<T>, T>();
            public readonly QueryDescription addDesc = new QueryDescription().WithAll<History<T>>().WithNone<T>();
            public readonly QueryDescription tickDesc = new QueryDescription().WithAll<History<T>, T>();
        }
        private readonly GenericCreator _desc = new();
        private readonly List<Entity> _entitiesCache = new();
        private readonly World _world;

        public RollbackHistorySystem(World world)
        {
            _world = world;
        }

        private static int WrapTick(int tick) => Constants.WrapTick(tick);

        public void Tick(int tick, fix deltaTime)
        {
            var action = new ApplyHistoryAction(_desc, _entitiesCache, _world, tick);
            HistoryComponentsRegistry.ForEachData(ref action);
        }


        private readonly struct ApplyHistoryAction : IStructGenericAction
        {
            private readonly GenericCreator _descriptions;
            private readonly List<Entity> _entities;
            private readonly World _world;
            private readonly int _tick;

            public ApplyHistoryAction(GenericCreator descriptions, List<Entity> entities, World world, int tick)
            {
                _descriptions = descriptions;
                _entities = entities;
                _world = world;
                _tick = tick;
            }

            public void Invoke<T>() where T : struct
            {
                var desc = _descriptions.Get<Description<T>>();
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
                if (history[WrapTick(_tick)].HasValue)
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
                if (!history[WrapTick(_tick)].HasValue)
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
                var cmp = history[WrapTick(_tick)];

                if (!cmp.HasValue)
                    throw new InvalidOperationException();

                component = cmp.Value;
            }
        }
    }
}
