using DVG.Commands;
using DVG.SkyPirates.Shared.Commands;
using DVG.SkyPirates.Shared.Components.Runtime;
using DVG.SkyPirates.Shared.IFactories;
using DVG.SkyPirates.Shared.IServices;

namespace DVG.SkyPirates.Shared.Services.CommandExecutors
{
    public class SpawnSquadCommandExecutor :
        ICommandExecutor<SpawnSquadCommand>
    {
        private readonly ISquadFactory _squadFactory;

        public SpawnSquadCommandExecutor(
            ISquadFactory squadFactory)
        {
            _squadFactory = squadFactory;
        }

        public void Execute(Command<SpawnSquadCommand> cmd)
        {
            Team team = new() { Id = cmd.ClientId };
            var squad = _squadFactory.Create((cmd.Data.CreationData, team));
        }
    }
}
