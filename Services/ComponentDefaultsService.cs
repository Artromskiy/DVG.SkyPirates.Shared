using Arch.Core;
using DVG.SkyPirates.Shared.Data;
using DVG.SkyPirates.Shared.IServices;
using System.Runtime.CompilerServices;

namespace DVG.SkyPirates.Shared.Services
{
    public class ComponentDefaultsService : IComponentDefaultsService
    {
        private readonly World _world;
        private readonly ComponentDefaultsConfig _componentDefaultsConfig;

        public ComponentDefaultsService(World world, ComponentDefaultsConfig componentDefaultsConfig)
        {
            _world = world;
            _componentDefaultsConfig = componentDefaultsConfig;
        }

        public void SetDefaults(Entity entity)
        {
            foreach (var item in _componentDefaultsConfig)
            {
                var setAction = new SetDefaultAction(_world, entity, item.Default);
                item.Default.ForEach(ref setAction);
            }
            foreach (var item in _componentDefaultsConfig)
            {
                var copyAction = new CopyAction(_world, entity, item.CopyTo);
                item.CopyFrom.ForEach(ref copyAction);
            }
        }

        private readonly struct SetDefaultAction : IStructGenericAction
        {
            private readonly World _world;
            private readonly Entity _entity;
            private readonly ComponentsSet _set;

            public SetDefaultAction(World world, Entity entity, ComponentsSet set)
            {
                _world = world;
                _entity = entity;
                _set = set;
            }

            public readonly void Invoke<T>() where T : struct
            {
                if (_world.Has<T>(_entity))
                {
                    var cmp = _set.Get<T>().Value;
                    _world.Set(_entity, cmp);
                }
            }
        }

        private readonly struct CopyAction : IStructGenericAction
        {
            private readonly World _world;
            private readonly Entity _entity;
            private readonly ComponentsMask _to;

            public CopyAction(World world, Entity entity, ComponentsMask to)
            {
                _world = world;
                _entity = entity;
                _to = to;
            }

            public readonly void Invoke<From>() where From : struct
            {
                if (_world.TryGet(_entity, out From cmp))
                {
                    var setAction = new SetAction<From>(_world, _entity, cmp);
                    _to.ForEach(ref setAction);
                }
            }

            private readonly struct SetAction<From> : IStructGenericAction
            {
                private readonly World _world;
                private readonly Entity _entity;
                private readonly From _from;

                public SetAction(World world, Entity entity, From from)
                {
                    _world = world;
                    _entity = entity;
                    _from = from;
                }

                public void Invoke<To>() where To : struct
                {
                    if (_world.Has<To>(_entity))
                    {
                        var cmpFrom = _from;
                        var cmpTo = Unsafe.As<From, To>(ref cmpFrom);
                        _world.Set(_entity, cmpTo);
                    }
                }
            }
        }
    }
}
