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
        private class Description<T>
        {
            public readonly QueryDescription Desc = new QueryDescription().WithAll<T, Disposing, Temp>();
        }
        private readonly QueryDescription _disposingDesc = new QueryDescription().WithAll<Disposing>();
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
            var initDisposing = new InitDiposing(tick);
            _world.InlineQuery<InitDiposing, Disposing>(in _disposingDesc, ref initDisposing);
            var selectToDispose = new SelectToDispose(tick, _entitiesCache);
            _world.InlineEntityQuery<SelectToDispose, Disposing>(in _disposingDesc, ref selectToDispose);
            foreach (var entity in _entitiesCache)
                _world.Add<Temp>(entity);

            var disposeCallAction = new DisposeCallAction(_world, _disposeDesc);
            DisposableComponentsRegistry.ForEachData(ref disposeCallAction);

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

            private readonly struct DisposeQuery<T> : IForEach<T> where T : struct, IDisposable
            {
                public readonly void Update(ref T component) => component.Dispose();
            }
        }

        private readonly struct InitDiposing : IForEach<Disposing>
        {
            private readonly int _tick;

            public InitDiposing(int tick)
            {
                _tick = tick;
            }

            public void Update(ref Disposing disposing)
            {
                if (disposing.StartTick == 0)
                    disposing.StartTick = _tick;
            }
        }

        private readonly struct SelectToDispose : IForEachWithEntity<Disposing>
        {
            private readonly int _tick;
            private readonly List<Entity> _entities;

            public SelectToDispose(int tick, List<Entity> entities)
            {
                _tick = tick;
                _entities = entities;
            }

            public void Update(Entity entity, ref Disposing disposing)
            {
                int disposeTick = disposing.StartTick + Constants.MaxHistoryTicks;
                if (_tick >= disposeTick)
                    _entities.Add(entity);
            }
        }
    }
}
