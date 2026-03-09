using Arch.Core;
using DVG.Collections;
using DVG.Components;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;
using System;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Systems.Special
{
    internal class DisposeSystem : IDisposeSystem
    {
        private class Description<T> where T : struct
        {
            public readonly QueryDescription Desc = new QueryDescription().WithAll<T, Temp>();
            public readonly QueryDescription HistoryDesc = new QueryDescription().WithAll<History<T>, Temp>();
        }

        private readonly QueryDescription _disposingDesc = new QueryDescription().
            WithAll<History<Alive>>().WithNone<Alive>();

        private readonly List<Entity> _entitiesCache = new();
        private readonly GenericCreator _disposeDesc = new();

        private readonly World _world;

        public DisposeSystem(World world)
        {
            _world = world;
        }

        public void Tick(int tick)
        {
            _entitiesCache.Clear();
            var selectToDispose = new SelectToDispose(_entitiesCache);
            _world.InlineEntityQuery<SelectToDispose, History<Alive>>(in _disposingDesc, ref selectToDispose);
            foreach (var entity in _entitiesCache)
                _world.Add<Temp>(entity);

            var componentsDispose = new ComponentsDisposeCallAction(_world, _disposeDesc);
            DisposableComponentsRegistry.ForEachData(ref componentsDispose);
            var historyDispose = new HistoryDisposeCallAction(_world, _disposeDesc);
            HistoryComponentsRegistry.ForEachData(ref historyDispose);

            _world.Destroy(new QueryDescription().WithAll<Temp>());
        }

        private readonly struct ComponentsDisposeCallAction : IStructGenericAction<IDisposable>
        {
            private readonly World _world;
            private readonly GenericCreator _desc;

            public ComponentsDisposeCallAction(World world, GenericCreator desc)
            {
                _world = world;
                _desc = desc;
            }

            public void Invoke<T>() where T : struct, IDisposable
            {
                var query = new DisposeQuery<T>();
                var desc = _desc.Get<Description<T>>().Desc;
                _world.InlineQuery<DisposeQuery<T>, T>(in desc, ref query);
            }

            private readonly struct DisposeQuery<T> : IForEach<T> where T : struct, IDisposable
            {
                public readonly void Update(ref T component) => component.Dispose();
            }
        }

        private readonly struct HistoryDisposeCallAction : IStructGenericAction
        {
            private readonly World _world;
            private readonly GenericCreator _desc;

            public HistoryDisposeCallAction(World world, GenericCreator desc)
            {
                _world = world;
                _desc = desc;
            }

            public void Invoke<T>() where T : struct
            {
                var query = new DisposeQuery<T>();
                var desc = _desc.Get<Description<T>>().HistoryDesc;
                _world.InlineQuery<DisposeQuery<T>, History<T>>(in desc, ref query);
            }

            private readonly struct DisposeQuery<T> : IForEach<History<T>> where T : struct
            {
                public readonly void Update(ref History<T> history) => history.Dispose();
            }
        }


        private readonly struct SelectToDispose : IForEachWithEntity<History<Alive>>
        {
            private readonly List<Entity> _entities;

            public SelectToDispose(List<Entity> entities)
            {
                _entities = entities;
            }

            public void Update(Entity entity, ref History<Alive> aliveHistory)
            {
                if (aliveHistory.Count != Constants.MaxHistoryTicks)
                    return;

                for (int i = 0; i < Constants.MaxHistoryTicks; i++)
                    if (aliveHistory._values[i].HasValue)
                        return;

                _entities.Add(entity);
            }
        }
    }
}
