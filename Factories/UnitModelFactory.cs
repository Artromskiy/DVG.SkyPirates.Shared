using DVG.Core;
using DVG.SkyPirates.Shared.Commands;
using DVG.SkyPirates.Shared.IFactories;
using DVG.SkyPirates.Shared.Configs;

namespace DVG.SkyPirates.Shared.Factories
{
    public class UnitModelFactory : IUnitConfigFactory
    {
        private readonly IPathFactory<UnitConfig> _pathFactory;

        public UnitModelFactory(IPathFactory<UnitConfig> pathFactory)
        {
            _pathFactory = pathFactory;
        }

        public UnitConfig Create(SpawnUnit parameters)
        {
            return _pathFactory.Create($"Configs/Units/{parameters.UnitId.value}");
        }
    }
}
