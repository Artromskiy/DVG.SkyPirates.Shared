using DVG.Core;
using DVG.SkyPirates.Shared.Commands;
using DVG.SkyPirates.Shared.IFactories;
using DVG.SkyPirates.Shared.Configs;

namespace DVG.SkyPirates.Shared.Factories
{
    public class UnitConfigFactory : IUnitConfigFactory
    {
        private readonly IPathFactory<UnitConfig> _pathFactory;

        public UnitConfigFactory(IPathFactory<UnitConfig> pathFactory)
        {
            _pathFactory = pathFactory;
        }

        public UnitConfig Create(SpawnUnitCommand parameters)
        {
            return _pathFactory.Create($"Configs/Units/{parameters.UnitId.Value}");
        }
    }
}
