using Delta.ECS;

namespace DVG.SkyPirates.Shared.IServices
{
    public interface IComponentDependenciesService
    {
        void AddDependencies(Entity entity);
    }
}