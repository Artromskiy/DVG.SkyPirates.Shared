using DVG.Core;
using DVG.SkyPirates.Shared.Commands;
using DVG.SkyPirates.Shared.IFactories;
using DVG.SkyPirates.Shared.IServices;

namespace DVG.SkyPirates.Shared.Services.CommandExecutors
{
    public class SpawnSquadCommandExecutor :
        ICommandExecutor<SpawnSquadCommand>
    {
        private readonly ISquadFactory _squadFactory;
        private readonly IEntitiesService _entitiesService;
        private readonly IOwnershipService _ownershipService;

        public SpawnSquadCommandExecutor(
            ISquadFactory squadFactory,
            IEntitiesService entitiesService,
            IOwnershipService ownershipService)
        {
            _squadFactory = squadFactory;
            _entitiesService = entitiesService;
            _ownershipService = ownershipService;
        }

        public void Execute(Command<SpawnSquadCommand> cmd)
        {
            var squad = _squadFactory.Create(cmd);
            if(!_entitiesService.HasEntity(cmd.EntityId))
                _entitiesService.AddEntity(cmd.EntityId, squad);
            _ownershipService.SetOwner(cmd.EntityId, cmd.ClientId);
        }
    }
}
