using Delta.ECS;
using DVG.SkyPirates.Shared.Data;
using DVG.SkyPirates.Shared.IServices;

namespace DVG.SkyPirates.Shared.Services
{
    public class ComponentDependenciesService : IComponentDependenciesService
    {
        private readonly World _world;
        private readonly RuntimeDependency[] _dependencies;

        public ComponentDependenciesService(World world, ComponentDependenciesConfig dependencies)
        {
            _world = world;
            _dependencies = new RuntimeDependency[dependencies.Count];
            for (int i = 0; i < _dependencies.Length; i++)
            {
                var dependency = dependencies[i];
                _dependencies[i] = new RuntimeDependency(dependency.Has.GetIds(world), dependency.Add.GetIds(world));
            }
        }

        public void AddDependencies(Entity entity)
        {
            foreach (var config in _dependencies)
            {
                bool hasAll = true;
                var required = config.Has;
                for (int i = 0; i < required.Length; i++)
                    hasAll &= _world.Has(entity, required[i]);
                if (!hasAll) continue;
                var added = config.Add;
                for (int i = 0; i < added.Length; i++)
                    _world.Add(entity, added[i]);
            }
        }

        private readonly struct RuntimeDependency
        {
            public readonly ComponentId[] Has;
            public readonly ComponentId[] Add;

            public RuntimeDependency(ComponentId[] has, ComponentId[] add)
            {
                Has = has;
                Add = add;
            }
        }
    }
}
