using DVG.Core;
using DVG.SkyPirates.Shared.Commands;
using DVG.SkyPirates.Shared.IFactories;
using DVG.SkyPirates.Shared.Entities;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Factories
{
    public class UnitFactory : IUnitFactory
    {
        private readonly IUnitViewFactory _unitViewFactory;
        private readonly IUnitConfigFactory _unitConfigFactory;
        private readonly IUnitViewModelFactory _unitViewModelFactory;

        private readonly Dictionary<int, UnitEntity> _unitsCache = new Dictionary<int, UnitEntity>();

        public UnitFactory(
            IUnitViewFactory unitViewFactory,
            IUnitConfigFactory unitConfigFactory,
            IUnitViewModelFactory unitViewModelFactory)
        {
            _unitViewFactory = unitViewFactory;
            _unitConfigFactory = unitConfigFactory;
            _unitViewModelFactory = unitViewModelFactory;
        }

        public UnitEntity Create(Command<SpawnUnit> cmd)
        {
            if (_unitsCache.TryGetValue(cmd.EntityId, out var unit))
                return unit;

            var view = _unitViewFactory.Create(cmd.Data);
            var config = _unitConfigFactory.Create(cmd.Data);
            
            unit = _unitsCache[cmd.EntityId] = new UnitEntity(config);
            var viewModel = _unitViewModelFactory.Create(unit);
            view.Inject(viewModel);

            return unit;
        }
    }
}