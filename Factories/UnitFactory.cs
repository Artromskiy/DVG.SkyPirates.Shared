using DVG.SkyPirates.Shared.Ids;
using DVG.SkyPirates.Shared.IFactories;
using DVG.SkyPirates.Shared.Presenters;

namespace DVG.SkyPirates.Shared.Factories
{
    public class UnitFactory : IUnitFactory
    {
        private readonly IUnitViewFactory _unitViewFactory;
        private readonly IUnitModelFactory _unitModelFactory;

        public UnitFactory(IUnitViewFactory unitViewFactory, IUnitModelFactory unitModelFactory)
        {
            _unitViewFactory = unitViewFactory;
            _unitModelFactory = unitModelFactory;
        }

        public UnitPm Create((UnitId unitId, int level, int merge) parameters)
        {
            var view = _unitViewFactory.Create(parameters);
            var model = _unitModelFactory.Create(parameters);
            UnitPm unit = new UnitPm(view, model);
            return unit;
        }
    }
}