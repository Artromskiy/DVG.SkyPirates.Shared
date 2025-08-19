using DVG.Core;
using DVG.SkyPirates.Shared.Commands;
using DVG.SkyPirates.Shared.IFactories;
using DVG.SkyPirates.Shared.Entities;
using System.Collections.Generic;
using Arch.Core;
using DVG.SkyPirates.Shared.Components;

namespace DVG.SkyPirates.Shared.Factories
{
    public class UnitFactory : IUnitFactory
    {
        private readonly IUnitConfigFactory _unitConfigFactory;

        private readonly Dictionary<int, Entity> _unitsCache = new Dictionary<int, Entity>();

        public UnitFactory(IUnitConfigFactory unitConfigFactory)
        {
            _unitConfigFactory = unitConfigFactory;
        }

        public Entity Create(Command<SpawnUnitCommand> cmd)
        {
            if (_unitsCache.TryGetValue(cmd.EntityId, out var unit))
                return unit;

            var config = _unitConfigFactory.Create(cmd.Data);

            _unitsCache[cmd.EntityId] = unit = EntityIds.Get(cmd.EntityId);

            unit.Add<
                Unit,
                Position,
                Rotation,
                Health,
                Team,
                Fixation>();

            return unit;
        }
    }
}