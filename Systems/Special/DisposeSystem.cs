using Arch.Core;
using DVG.Collections;
using DVG.Components;
using DVG.SkyPirates.Shared.IServices;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;
using System;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Systems.Special
{
    internal class DisposeSystem : IDisposeSystem
    {
        private class Description<T>
        {
            public readonly QueryDescription Desc = new QueryDescription().WithAll<T, Disposing, Temp>();
        }
        private readonly QueryDescription _descSearch = new QueryDescription().WithAll<Disposing>();
        private readonly List<Entity> _entitiesCache = new();
        private readonly GenericCreator _disposeDesc = new();

        private readonly IPooledItemsProvider _pooledItemsProvider;
        private readonly World _world;

        public DisposeSystem(IPooledItemsProvider pooledItemsProvider, World world)
        {
            _pooledItemsProvider = pooledItemsProvider;
            _world = world;
        }

        public void Tick(int tick, fix deltaTime)
        {
            _entitiesCache.Clear();
            var query = new SelectToDispose(_entitiesCache);
            _world.InlineEntityQuery<SelectToDispose, Disposing>(_descSearch, ref query);
            foreach (var entity in _entitiesCache)
                _world.Add<Temp>(entity);

            var disposeCallAction = new DisposeCallAction(_world, _disposeDesc, _pooledItemsProvider);
            DisposableComponentsRegistry.ForEachData(ref disposeCallAction);

            _world.Destroy(new QueryDescription().WithAll<Temp>());
        }

        private readonly struct DisposeCallAction : IStructGenericAction<IDisposable>
        {
            private readonly World _world;
            private readonly GenericCreator _desc;
            private readonly IPooledItemsProvider _pooledItemsProvider;

            public DisposeCallAction(World world, GenericCreator desc, IPooledItemsProvider pooledItemsProvider)
            {
                _world = world;
                _desc = desc;
                _pooledItemsProvider = pooledItemsProvider;
            }

            public void Invoke<T>() where T : struct, IDisposable
            {
                var query = new DisposeQuery<T>(_pooledItemsProvider);
                var desc = _desc.Get<Description<T>>().Desc;
                _world.InlineQuery<DisposeQuery<T>, T>(in desc, ref query);
            }

            private readonly struct DisposeQuery<T> : IForEach<T> where T : struct, IDisposable
            {
                private readonly IPooledItemsProvider _pooledItemsProvider;

                public DisposeQuery(IPooledItemsProvider pooledItemsProvider)
                {
                    _pooledItemsProvider = pooledItemsProvider;
                }

                public readonly void Update(ref T component)
                {
                    _pooledItemsProvider.Return(component);
                }
            }
        }

        private readonly struct SelectToDispose : IForEachWithEntity<Disposing>
        {
            private readonly List<Entity> _entities;

            public SelectToDispose(List<Entity> entities)
            {
                _entities = entities;
            }

            public void Update(Entity entity, ref Disposing destruct)
            {
                if (++destruct.TicksPassed > Constants.HistoryTicks)
                    _entities.Add(entity);
            }
        }

    }
}
