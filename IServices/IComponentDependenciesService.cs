using Arch.Core;

namespace DVG.SkyPirates.Shared.IServices
{
    public interface IComponentDependenciesService
    {
        void AddDependencies(Entity entity);
    }
}