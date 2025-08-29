using DVG.Core;
using DVG.SkyPirates.Shared.Commands;
using DVG.SkyPirates.Shared.IFactories;
using DVG.SkyPirates.Shared.Configs;
using DVG.SkyPirates.Shared.Ids;

namespace DVG.SkyPirates.Shared.Factories
{
    public class UnitConfigFactory : IUnitConfigFactory
    {
        private readonly IPathFactory<UnitConfig> _pathFactory;

        public UnitConfigFactory(IPathFactory<UnitConfig> pathFactory)
        {
            _pathFactory = pathFactory;
        }

        public UnitConfig Create(UnitId unitId)
        {
            return _pathFactory.Create($"Configs/Units/{unitId.Value}");
        }
    }
}
