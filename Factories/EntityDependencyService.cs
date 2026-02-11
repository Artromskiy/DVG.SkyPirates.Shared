using Arch.Core;
using DVG.SkyPirates.Shared.Data;
using DVG.SkyPirates.Shared.IFactories;

namespace DVG.SkyPirates.Shared.Factories
{
    public class EntityDependencyService : IEntityDependencyService
    {
        private readonly World _world;
        private readonly EnsureConfig[] _dependencies;

        public EntityDependencyService(World world, IGlobalConfigFactory globalConfigFactory)
        {
            _world = world;
            _dependencies = globalConfigFactory.Create().ComponentDependencies;
        }

        public void EnsureDependencies(Entity entity)
        {
            foreach (var config in _dependencies)
            {
                var hasAll = new HasAllAction(_world, entity);
                config.Has.ForEach(ref hasAll);
                if (!hasAll.Value)
                    continue;

                var add = new AddAction(config.DefaultOnAdd, _world, entity);
                config.Add.ForEach(ref add);
            }
        }

        private struct HasAllAction : IStructGenericAction
        {
            public bool Value { get; private set; }
            private readonly World _world;
            private readonly Entity _entity;

            public HasAllAction(World world, Entity entity) : this()
            {
                Value = true;
                _world = world;
                _entity = entity;
            }

            public void Invoke<T>() where T : struct
            {
                Value &= _world.Has<T>(_entity);
            }
        }

        private readonly struct AddAction : IStructGenericAction
        {
            private readonly ComponentsData _defaults;
            private readonly World _world;
            private readonly Entity _entity;

            public AddAction(ComponentsData defaults, World world, Entity entity)
            {
                _defaults = defaults;
                _world = world;
                _entity = entity;
            }

            public void Invoke<T>() where T : struct
            {
                T defaultValue = _defaults?.Get<T>() ?? default;
                _world.AddOrGet<T>(_entity) = defaultValue;
            }
        }
    }
}
