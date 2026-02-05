using Arch.Core;
using DVG.Core;
using DVG.Core.Components;

namespace DVG.SkyPirates.Shared.Components.Special
{
    public static class WorldExt
    {
        public static ref T GetSingleton<T>(this World world)
        {
            if (!world.TryGetArchetype(Component<T>.Signature, out var archetype))
            {
                var entity = world.Create<T>();
                archetype = world.GetArchetype(entity);
            }
            return ref archetype.GetChunk(0).GetFirst<T>();
        }

        public static void AddQuery<Has, Add>(this World world, ForEach<Has, Add> forEach)
        {
            var addDesc = new QueryDescription().WithAll<Has>().WithNone<Add>();
            world.Add<Add, Temp>(addDesc);
            var queryDesc = new QueryDescription().WithAll<Add, Temp>();
            world.Query(queryDesc, forEach);
            world.Remove<Temp>(queryDesc);
        }

        public static void SetConfig(this World world, Entity entity, Data.EntityData config)
        {
            var action = new ApplyComponent(entity, world, config);
            ComponentIds.ForEachData(ref action);
        }

        private readonly struct ApplyComponent : IStructGenericAction
        {
            private readonly Entity _entity;
            private readonly World _world;
            private readonly Data.EntityData _config;

            public ApplyComponent(Entity entity, World world, Data.EntityData config)
            {
                _entity = entity;
                _world = world;
                _config = config;
            }

            public void Invoke<T>() where T : struct
            {
                var cmp = _config.Get<T>();
                if (cmp.HasValue && !_world.Has<T>(_entity))
                    _world.Add<T>(_entity);
                if (cmp.HasValue)
                    _world.Set(_entity, cmp.Value);
            }
        }
    }
}
