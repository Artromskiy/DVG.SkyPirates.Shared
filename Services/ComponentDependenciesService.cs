using Arch.Core;
using DVG.SkyPirates.Shared.Data;
using DVG.SkyPirates.Shared.IServices;

namespace DVG.SkyPirates.Shared.Services
{
    public class ComponentDependenciesService : IComponentDependenciesService
    {
        private readonly World _world;
        private readonly ComponentDependenciesConfig _dependencies;

        public ComponentDependenciesService(World world, ComponentDependenciesConfig dependencies)
        {
            _world = world;
            _dependencies = dependencies;
        }

        public void AddDependencies(Entity entity)
        {
            foreach (var config in _dependencies)
            {
                var hasAll = new HasAllAction(_world, entity);
                config.Has.ForEach(ref hasAll);
                if (!hasAll.Value)
                    continue;

                var add = new AddAction(_world, entity);
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
            private readonly World _world;
            private readonly Entity _entity;

            public AddAction(World world, Entity entity)
            {
                _world = world;
                _entity = entity;
            }

            public void Invoke<T>() where T : struct
            {
                _world.AddOrGet<T>(_entity) = default;
            }
        }
    }
}
