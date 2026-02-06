using DVG.Core;
using DVG.SkyPirates.Shared.Data;

namespace DVG.SkyPirates.Shared.IFactories
{
    public interface IWorldDataFactory : IFactory<WorldData>
    {
        void Extract(WorldData worldData);
    }
}
