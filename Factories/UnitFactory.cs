using Arch.Core;
using Arch.Core.Extensions;
using DVG.Core;
using DVG.SkyPirates.Shared.Archetypes;
using DVG.SkyPirates.Shared.Commands;
using DVG.SkyPirates.Shared.Components;
using DVG.SkyPirates.Shared.Entities;
using DVG.SkyPirates.Shared.IFactories;

namespace DVG.SkyPirates.Shared.Factories
{
    public class UnitFactory : IUnitFactory
    {
        private readonly IUnitConfigFactory _unitConfigFactory;

        public UnitFactory(IUnitConfigFactory unitConfigFactory)
        {
            _unitConfigFactory = unitConfigFactory;
        }

        public virtual Entity Create(Command<SpawnUnitCommand> cmd)
        {
            var config = _unitConfigFactory.Create(cmd.Data);

            var unit = EntityIds.Get(cmd.EntityId);

            UnitArch.EnsureArch(unit);
            HistoryArch.EnsureHistory(unit);

            unit.Get<Team>().Id = cmd.ClientId;
            unit.Get<Health>().Value = config.health;
            unit.Get<Damage>().Value = config.damage;
            unit.Get<MoveSpeed>().Value = config.speed;
            unit.Get<ImpactDistance>().Value = config.attackDistance;

            unit.Get<Creation>() = new Creation(cmd.Tick);

            return unit;
        }
    }
}