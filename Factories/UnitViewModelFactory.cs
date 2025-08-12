using DVG.SkyPirates.Shared.Entities;
using DVG.SkyPirates.Shared.IFactories;
using DVG.SkyPirates.Shared.IViewModels;
using DVG.SkyPirates.Shared.ViewModels;

namespace DVG.SkyPirates.Shared.Factories
{
    public class UnitViewModelFactory : IUnitViewModelFactory
    {
        public IUnitVM Create(UnitEntity parameters)
        {
            return new UnitVM(parameters);
        }
    }
}
