using DVG.Commands;
using DVG.SkyPirates.Shared.Commands;
using DVG.SkyPirates.Shared.IFactories;
using DVG.SkyPirates.Shared.IServices;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;

namespace DVG.SkyPirates.Shared.Services.CommandExecutors
{
    public class LoadWorldCommandExecutor : ICommandExecutor<LoadWorldCommand>
    {
        private readonly IWorldDataFactory _worldDataFactory;
        private readonly IHistorySystem _historySystem;

        public LoadWorldCommandExecutor(IWorldDataFactory worldDataFactory, IHistorySystem historySystem)
        {
            _worldDataFactory = worldDataFactory;
            _historySystem = historySystem;
        }

        public void Execute(Command<LoadWorldCommand> cmd)
        {
            _worldDataFactory.Extract(cmd.Data.WorldData);
            _historySystem.Save(cmd.Tick - 1);
        }
    }
}
