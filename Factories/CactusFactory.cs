using Arch.Core;
using DVG.SkyPirates.Shared.Archetypes;
using DVG.SkyPirates.Shared.Components;
using DVG.SkyPirates.Shared.Components.Config;
using DVG.SkyPirates.Shared.Entities;
using DVG.SkyPirates.Shared.Ids;
using DVG.SkyPirates.Shared.IFactories;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Factories
{
    public class CactusFactory : ICactusFactory
    {
        private readonly World _world;

        public CactusFactory(World world)
        {
            _world = world;
        }

        public Entity Create((CactusId CactusId, int EntityId) parameters)
        {
            var entity = EntityIds.Get(parameters.EntityId);
            CactusArch.EnsureArch(_world, entity);

            _world.Get<CactusId>(entity) = parameters.CactusId;
            _world.Get<Radius>(entity).Value = fix.One / 2;
            _world.Get<Separation>(entity).AddRadius = fix.One / 2;
            _world.Get<Separation>(entity).AffectingCoeff = 1;
            _world.Get<Separation>(entity).AffectedCoeff = 0;
            _world.Get<Damage>(entity).Value = 1;
            _world.Get<ImpactDistance>(entity).Value = 1;
            _world.Get<TargetSearchData>(entity).Distance = 1;

            _world.Get<BehaviourConfig>(entity).Scenario = GetScenario();
            _world.Get<BehaviourConfig>(entity).Durations = GetConfigDurations();
            return entity;
        }

        private Dictionary<StateId, StateId> GetScenario() => new()
        {
            [StateId.Constants.PreAttack] = StateId.None,
            [StateId.None] = StateId.None,
        };

        private Dictionary<StateId, fix> GetConfigDurations() => new()
        {
            [StateId.Constants.PreAttack] = (fix)0.4m,
            [StateId.None] = 0,
        };
    }
}
