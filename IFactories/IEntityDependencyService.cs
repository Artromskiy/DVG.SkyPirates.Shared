using Arch.Core;

namespace DVG.SkyPirates.Shared.IFactories
{
    public interface IEntityDependencyService
    {
        void EnsureDependencies(Entity entity);
    }
}
