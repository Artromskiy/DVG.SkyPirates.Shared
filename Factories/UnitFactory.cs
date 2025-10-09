using Arch.Core;
using DVG.SkyPirates.Shared.Archetypes;
using DVG.SkyPirates.Shared.Components;
using DVG.SkyPirates.Shared.Components.Data;
using DVG.SkyPirates.Shared.Data;
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

        public UnitFactory(World world, IUnitConfigFactory unitConfigFactory)
        {
            _world = world;
            _unitConfigFactory = unitConfigFactory;
        }

        public virtual Entity Create((UnitId UnitId, int EntityId) parameters)
        {
            var config = _unitConfigFactory.Create(parameters.UnitId);

            var entity = EntityIds.Get(parameters.EntityId);

            UnitArch.EnsureArch(_world, entity);

            _world.Get<UnitId>(entity) = parameters.UnitId;

            _world.Get<Health>(entity).Value = config.health;
            _world.Get<MaxHealth>(entity).Value = config.health;
            _world.Get<Damage>(entity).Value = config.damage;
            _world.Get<MoveSpeed>(entity).Value = config.speed;
            _world.Get<ImpactDistance>(entity).Value = config.attackDistance;

            _world.Get<CircleShape>(entity).Radius = fix.One / 3;
            _world.Get<Separation>(entity).AddRadius = fix.One / 3;
            _world.Get<Separation>(entity).AffectingCoeff = 1;
            _world.Get<Separation>(entity).AffectedCoeff = 1;
            _world.Get<AutoHeal>(entity).healDelay = 10;
            _world.Get<AutoHeal>(entity).healPerSecond = 20;

            _world.Get<BehaviourConfig>(entity).Scenario = GetScenario();
            _world.Get<BehaviourConfig>(entity).Durations = GetConfigDurations(config);

            return entity;
        }

        private Dictionary<StateId, StateId> GetScenario() => new()
        {
            [StateId.Constants.PreAttack] = StateId.Constants.PostAttack,
            [StateId.Constants.PostAttack] = StateId.Constants.Reload,
            [StateId.Constants.Reload] = StateId.None,
            [StateId.None] = StateId.None,
        };

        private Dictionary<StateId, fix> GetConfigDurations(UnitConfig config) => new()
        {
            [StateId.Constants.PreAttack] = config.preAttack,
            [StateId.Constants.PostAttack] = config.postAttack,
            [StateId.Constants.Reload] = config.reload,
            [StateId.None] = 0,
        };
    }
}