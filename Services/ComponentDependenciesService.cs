using Delta.ECS;
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
                bool hasAll = true;
                foreach (var required in config.Has)
                    hasAll &= _world.Has(entity, required.ComponentId);
                if (!hasAll) continue;
                foreach (var added in config.Add)
                    _world.Add(entity, added.ComponentId);
            }
        }
    }
}
