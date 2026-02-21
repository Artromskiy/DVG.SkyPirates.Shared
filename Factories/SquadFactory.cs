using Arch.Core;
using DVG.SkyPirates.Shared.Components.Runtime;
using DVG.SkyPirates.Shared.Data;
using DVG.SkyPirates.Shared.IFactories;
using DVG.SkyPirates.Shared.Systems;

namespace DVG.SkyPirates.Shared.Factories
{
    public class SquadFactory : ISquadFactory
    {
        private readonly SquadStatsConfig _squadStats;
        private readonly IEntityDependencyService _entityDependencyService;
        private readonly IEntityFactory _commandEntityFactory;
        private readonly World _world;

        public SquadFactory(SquadStatsConfig squadStats, IEntityDependencyService entityDependencyService, IEntityFactory commandEntityFactory, World world)
        {
            _squadStats = squadStats;
            _entityDependencyService = entityDependencyService;
            _commandEntityFactory = commandEntityFactory;
            _world = world;
        }

        public Entity Create((EntityParameters entityParameters, TeamId team) parameters)
        {
            var entity = _commandEntityFactory.Create(parameters.entityParameters);
            _world.Add<Squad>(entity);
            _entityDependencyService.EnsureDependencies(entity);
            _world.SetEntityData(entity, _squadStats[0]);
            _world.Get<TeamId>(entity) = parameters.team;
            return entity;
        }
    }
}
