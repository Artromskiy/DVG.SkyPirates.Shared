using DVG.Core;
using DVG.SkyPirates.Shared.Commands;
using DVG.SkyPirates.Shared.IServices;

namespace DVG.SkyPirates.Shared.Services.CommandExecutors
{
    public class EntityCommandExecutor :
        ICommandExecutor<DirectionCommand>,
        ICommandExecutor<FixationCommand>
    {
        private readonly IEntitiesService _entitiesService;

        public EntityCommandExecutor(IEntitiesService entitiesService)
        {
            _entitiesService = entitiesService;
        }

        public void Execute(Command<FixationCommand> cmd) => ExecuteCommand(cmd);

        public void Execute(Command<DirectionCommand> cmd) => ExecuteCommand(cmd);

        private void ExecuteCommand<T>(Command<T> cmd)
            where T: ICommandData
        {
            if (_entitiesService.TryGetEntity<ICommandable<T>>(cmd.EntityId, out var entity))
                entity.Execute(cmd.Data);
        }
    }
}
