using Arch.Core;
using Arch.Core.Extensions;
using DVG.Core;
using DVG.SkyPirates.Shared.Archetypes;
using DVG.SkyPirates.Shared.Commands;
using DVG.SkyPirates.Shared.Components;
using DVG.SkyPirates.Shared.Components.Data;
using DVG.SkyPirates.Shared.Entities;
using DVG.SkyPirates.Shared.Ids;
using DVG.SkyPirates.Shared.IFactories;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Factories
{
    public class UnitFactory : IUnitFactory
    {
        private readonly IUnitConfigFactory _unitConfigFactory;

        private readonly Dictionary<StateId, StateId> switchTable = new Dictionary<StateId, StateId>()
        {
            [StateId.Constants.PreAttack] = StateId.Constants.PostAttack,
            [StateId.Constants.PostAttack] = StateId.Constants.Reload,
            [StateId.Constants.Reload] = StateId.None,
            [StateId.None] = StateId.None,
        };

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
            unit.Get<BehaviourConfig>().Scenario = switchTable;
            unit.Get<PositionSeparation>().Radius = (fix)1 / 2;
            unit.Get<PositionSeparation>().Weight = 1;
            unit.Get<BehaviourConfig>().Durations = new Dictionary<StateId, fix>()
            {
                [StateId.Constants.PreAttack] = config.preAttack,
                [StateId.Constants.PostAttack] = config.postAttack,
                [StateId.Constants.Reload] = config.reload,
                [StateId.None] = 0,
            };
            return unit;
        }
    }
}