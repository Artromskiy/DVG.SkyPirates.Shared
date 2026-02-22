using DVG.Commands;
using DVG.SkyPirates.Shared.Commands;
using DVG.SkyPirates.Shared.IFactories;
using DVG.SkyPirates.Shared.IServices;

namespace DVG.SkyPirates.Shared.Services.CommandExecutors
{
    public class LoadWorldCommandExecutor : ICommandExecutor<LoadWorldCommand>
    {
        private readonly IWorldDataFactory _worldDataFactory;

        public LoadWorldCommandExecutor(IWorldDataFactory worldDataFactory)
        {
            _worldDataFactory = worldDataFactory;
        }

        public void Execute(Command<LoadWorldCommand> cmd)
        {
            _worldDataFactory.Extract(cmd.Data.WorldData);
        }
    }
}
