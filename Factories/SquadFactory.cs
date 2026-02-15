using Arch.Core;
using DVG.SkyPirates.Shared.Components.Config;
using DVG.SkyPirates.Shared.Components.Runtime;
using DVG.SkyPirates.Shared.Data;
using DVG.SkyPirates.Shared.IFactories;

namespace DVG.SkyPirates.Shared.Factories
{
    public class SquadFactory : ISquadFactory
    {
        private readonly IEntityDependencyService _entityDependencyService;
        private readonly IEntityFactory _commandEntityFactory;
        private readonly World _world;

        public SquadFactory(IEntityDependencyService entityDependencyService, IEntityFactory commandEntityFactory, World world)
        {
            _entityDependencyService = entityDependencyService;
            _commandEntityFactory = commandEntityFactory;
            _world = world;
        }

        public Entity Create((EntityParameters entityParameters, TeamId team) parameters)
        {
            var entity = _commandEntityFactory.Create(parameters.entityParameters);
            _world.Add<Squad>(entity);
            _entityDependencyService.EnsureDependencies(entity);
            _world.Get<Radius>(entity) = fix.One / 3;
            _world.Get<MaxSpeed>(entity) = (fix)7;
            _world.Get<TeamId>(entity) = parameters.team;
            return entity;
        }
    }
}
