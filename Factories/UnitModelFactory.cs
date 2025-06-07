using DVG.Core;
using DVG.SkyPirates.Shared.Commands;
using DVG.SkyPirates.Shared.IFactories;
using DVG.SkyPirates.Shared.Models;

namespace DVG.SkyPirates.Shared.Factories
{
    public class UnitModelFactory : IUnitModelFactory
    {
        private readonly IPathFactory<UnitModel> _pathFactory;

        public UnitModelFactory(IPathFactory<UnitModel> pathFactory)
        {
            _pathFactory = pathFactory;
        }

        public UnitModel Create(SpawnUnit parameters)
        {
            return _pathFactory.Create($"Configs/Units/{parameters.UnitId.value}");
        }

        public void Dispose(UnitModel instance) => _pathFactory.Dispose(instance);
    }
}
