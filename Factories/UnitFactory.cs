using DVG.Core;
using DVG.SkyPirates.Shared.Commands;
using DVG.SkyPirates.Shared.IFactories;
using DVG.SkyPirates.Shared.Presenters;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Factories
{
    public class UnitFactory : IUnitFactory
    {
        private readonly IUnitViewFactory _unitViewFactory;
        private readonly IUnitModelFactory _unitModelFactory;

        private readonly Dictionary<int, UnitPm> _unitsCache = new Dictionary<int, UnitPm>();

        public UnitFactory(
            IUnitViewFactory unitViewFactory,
            IUnitModelFactory unitModelFactory)
        {
            _unitViewFactory = unitViewFactory;
            _unitModelFactory = unitModelFactory;
        }

        public UnitPm Create(Command<SpawnUnit> cmd)
        {
            if (_unitsCache.TryGetValue(cmd.EntityId, out var unit))
                return unit;
            var view = _unitViewFactory.Create(cmd.Data);
            var model = _unitModelFactory.Create(cmd.Data);
            return _unitsCache[cmd.EntityId] = new UnitPm(view, model);
        }
    }
}