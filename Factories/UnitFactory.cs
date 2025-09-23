using Arch.Core;
using DVG.SkyPirates.Shared.Archetypes;
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
        private readonly World _world;

        private readonly Dictionary<StateId, StateId> switchTable = new Dictionary<StateId, StateId>()
        {
            [StateId.Constants.PreAttack] = StateId.Constants.PostAttack,
            [StateId.Constants.PostAttack] = StateId.Constants.Reload,
            [StateId.Constants.Reload] = StateId.None,
            [StateId.None] = StateId.None,
        };

        public UnitFactory(World world, IUnitConfigFactory unitConfigFactory)
        {
            _world = world;
            _unitConfigFactory = unitConfigFactory;
        }

        public virtual Entity Create((fix3 Position, int TeamId, int Level, UnitId UnitId, int EntityId) parameters)
        {
            var config = _unitConfigFactory.Create(parameters.UnitId);

            var unit = EntityIds.Get(parameters.EntityId);

            UnitArch.EnsureArch(_world, unit);
            HistoryArch.EnsureHistory(_world, unit);

            _world.Get<CircleShape>(unit).Radius = fix.One / 3;
            _world.Get<UnitId>(unit) = parameters.UnitId;
            _world.Get<Position>(unit).Value = parameters.Position;
            _world.Get<Team>(unit).Id = parameters.TeamId;
            _world.Get<Health>(unit).Value = config.health;
            _world.Get<MaxHealth>(unit).Value = config.health;
            _world.Get<Damage>(unit).Value = config.damage;
            _world.Get<MoveSpeed>(unit).Value = config.speed;
            _world.Get<ImpactDistance>(unit).Value = config.attackDistance;
            _world.Get<BehaviourConfig>(unit).Scenario = switchTable;
            _world.Get<Separation>(unit).AddRadius = fix.One / 3;
            _world.Get<Separation>(unit).AffectingCoeff = 1;
            _world.Get<Separation>(unit).AffectedCoeff = 1;
            _world.Get<AutoHeal>(unit).healDelay = 10;
            _world.Get<AutoHeal>(unit).healPerSecond = 20;

            _world.Get<BehaviourConfig>(unit).Durations = new Dictionary<StateId, fix>()
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