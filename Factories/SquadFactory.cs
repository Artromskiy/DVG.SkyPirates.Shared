using Arch.Core;
using DVG.Commands;
using DVG.Components;
using DVG.SkyPirates.Shared.Commands;
using DVG.SkyPirates.Shared.Components.Config;
using DVG.SkyPirates.Shared.Components.Runtime;
using DVG.SkyPirates.Shared.Data;
using DVG.SkyPirates.Shared.IFactories;

namespace DVG.SkyPirates.Shared.Factories
{
    public class SquadFactory : ISquadFactory
    {
        private readonly IEntityDependencyFactory _entityDependencyFactory;
        private readonly IEntityFactory _commandEntityFactory;
        private readonly World _world;

        public SquadFactory(IEntityDependencyFactory entityDependencyFactory, IEntityFactory commandEntityFactory, World world)
        {
            _entityDependencyFactory = entityDependencyFactory;
            _commandEntityFactory = commandEntityFactory;
            _world = world;
        }

        public Entity Create(Command<SpawnSquadCommand> cmd)
        {
            SyncId syncId = new() { Value = cmd.EntityId };
            var entity = _commandEntityFactory.Create(new EntityParameters(syncId, default, default));
            _world.Add<Squad>(entity);
            entity = _entityDependencyFactory.Create(entity);
            _world.Get<Radius>(entity).Value = fix.One / 3;
            _world.Get<MaxSpeed>(entity).Value = 7;
            _world.Get<Team>(entity).Id = cmd.ClientId;
            return entity;
        }
    }
}
