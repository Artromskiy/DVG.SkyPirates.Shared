using DVG.Core;
using DVG.SkyPirates.Shared.Commands.Lifecycle;
using DVG.SkyPirates.Shared.IFactories;
using DVG.SkyPirates.Shared.Presenters;

namespace DVG.SkyPirates.Shared.Factories
{
    public class UnitFactory : IUnitFactory
    {
        private readonly IUnitViewFactory _unitViewFactory;
        private readonly IUnitModelFactory _unitModelFactory;

        private readonly IPlayerLoopSystem _playerLoopSystem;

        public UnitFactory(
            IUnitViewFactory unitViewFactory,
            IUnitModelFactory unitModelFactory,
            IPlayerLoopSystem playerLoopSystem)
        {
            _unitViewFactory = unitViewFactory;
            _unitModelFactory = unitModelFactory;
            _playerLoopSystem = playerLoopSystem;
        }

        public UnitPm Create(SpawnUnit parameters)
        {
            var view = _unitViewFactory.Create(parameters);
            var model = _unitModelFactory.Create(parameters);
            UnitPm unit = new UnitPm(view, model);

            _playerLoopSystem.Add(unit);

            return unit;
        }

        public void Dispose(UnitPm instance)
        {
            _unitViewFactory.Dispose(instance.View);
            _unitModelFactory.Dispose(instance.Model);

            _playerLoopSystem.Remove(instance);
        }
    }
}