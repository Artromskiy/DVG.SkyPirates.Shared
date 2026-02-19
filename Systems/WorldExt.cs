using Arch.Core;
using DVG.Collections;
using DVG.Components;
using DVG.SkyPirates.Shared.Components.Framed;
using DVG.SkyPirates.Shared.Data;

namespace DVG.SkyPirates.Shared.Systems
{
    public static class WorldExt
    {
        private class WithAll<T>
        {
            public QueryDescription Desc = new QueryDescription().WithAll<T>();
        }
        private class WithAll<T0, T1>
        {
            public QueryDescription Desc = new QueryDescription().WithAll<T0, T1>();
        }
        private class WithAllWithNone<All, None>
        {
            public QueryDescription Desc = new QueryDescription().WithAll<All>().WithNone<None>();
        }

        private static readonly GenericCreator _desc = new();

        public static QueryDescription NotDisposing(this QueryDescription desc)
        {
            var none = Signature.Add(desc.None, Component<Disposing>.Signature);
            return new QueryDescription(desc.All, desc.Any, none, desc.Exclusive);
        }

        public static QueryDescription NotDisabled(this QueryDescription desc)
        {
            var none = Signature.Add(desc.None, Component<Disabled>.Signature);
            return new QueryDescription(desc.All, desc.Any, none, desc.Exclusive);
        }

        public static T FirstOrDefault<T>(this World world) where T : struct
        {
            var query = new FirstOrDefaultQuery<T>();
            var desc = _desc.Get<WithAll<T>>().Desc;
            world.InlineEntityQuery<FirstOrDefaultQuery<T>, T>(in desc, ref query);
            return query.Value;
        }

        public static Entity FirstOrDefaultEntity<T>(this World world) where T : struct
        {
            var query = new FirstOrDefaultQuery<T>();
            var desc = _desc.Get<WithAll<T>>().Desc;
            world.InlineEntityQuery<FirstOrDefaultQuery<T>, T>(in desc, ref query);
            return query.Entity;
        }

        public static void AddQuery<Has, Add>(this World world, ForEach<Has, Add> forEach)
        {
            var addDesc = _desc.Get<WithAllWithNone<Has, Add>>().Desc;
            if (world.CountEntities(addDesc) == 0)
                return;
            var queryDesc = _desc.Get<WithAll<Add, Temp>>().Desc;
            world.Add<Add, Temp>(addDesc);
            world.Query(queryDesc, forEach);
            world.Remove<Temp>(queryDesc);
        }

        public static void SetEntityData(this World world, Entity entity, ComponentsData config)
        {
            var action = new ApplyEntityData(entity, world, config);
            ComponentsRegistry.ForEachData(ref action);
        }

        private struct FirstOrDefaultQuery<T> : IForEachWithEntity<T>
        {
            public T Value;
            public Entity Entity;

            private bool _valueSet;
            public void Update(Entity e, ref T t)
            {
                if (_valueSet)
                    return;

                _valueSet = true;
                Value = t;
                Entity = e;
            }
        }

        private readonly struct ApplyEntityData : IStructGenericAction
        {
            private readonly Entity _entity;
            private readonly World _world;
            private readonly ComponentsData _config;

            public ApplyEntityData(Entity entity, World world, ComponentsData config)
            {
                _entity = entity;
                _world = world;
                _config = config;
            }

            public void Invoke<T>() where T : struct
            {
                var cmp = _config.Get<T>();
                if (cmp.HasValue)
                    _world.AddOrGet<T>(_entity) = cmp.Value;
            }
        }
    }
}
