using Arch.Core;
using DVG.Collections;
using DVG.Components;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;
using System;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Systems.Special
{
    internal class DisposeSystem : ITickableExecutor
    {
        private class Description<T>
        {
            public readonly QueryDescription Desc = new QueryDescription().WithAll<T, Dispose, Temp>();
        }
        private readonly QueryDescription _descSearch = new QueryDescription().WithAll<Dispose>();
        private readonly List<Entity> _entitiesCache = new();
        private readonly World _world;
        private readonly GenericCreator _disposeDesc = new();

        public DisposeSystem(World world)
        {
            _world = world;
        }

        public void Tick(int tick, fix deltaTime)
        {
            _entitiesCache.Clear();
            var query = new SelectToDispose(_entitiesCache);
            _world.InlineEntityQuery<SelectToDispose, Dispose>(_descSearch, ref query);
            foreach (var entity in _entitiesCache)
                _world.Add<Temp>(entity);

            var disposeCallAction = new DisposeCallAction(_world, _disposeDesc);
            //disposeCallAction.Invoke<>();

            _world.Destroy(new QueryDescription().WithAll<Temp>());
        }

        private readonly struct DisposeCallAction : IStructGenericAction<IDisposable>
        {
            private readonly World _world;
            private readonly GenericCreator _desc;

            public DisposeCallAction(World world, GenericCreator desc)
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

            private struct DisposeQuery<T> : IForEach<T> where T : IDisposable
            {
                public readonly void Update(ref T component) => component.Dispose();
            }
        }

        private readonly struct SelectToDispose : IForEachWithEntity<Dispose>
        {
            private readonly List<Entity> _entities;

            public SelectToDispose(List<Entity> entities)
            {
                _entities = entities;
            }

            public void Update(Entity entity, ref Dispose destruct)
            {
                if (++destruct.TicksPassed > Constants.HistoryTicks)
                    _entities.Add(entity);
            }
        }
    }
}
