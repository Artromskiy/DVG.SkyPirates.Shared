using DVG.Commands;
using DVG.SkyPirates.Shared.Commands;
using DVG.SkyPirates.Shared.IServices;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;

namespace DVG.SkyPirates.Shared.Services.CommandExecutors
{
    public class LoadWorldCommandExecutor : ICommandExecutor<LoadWorldCommand>
    {
        private readonly IHistorySystem _historySystem;

        public LoadWorldCommandExecutor(IHistorySystem historySystem)
        {
            _historySystem = historySystem;
        }

        public void Execute(Command<LoadWorldCommand> cmd)
        {
            _historySystem.ApplySnapshot(cmd.Data.WorldData);
            _historySystem.Save(cmd.Tick - 1);
        }
    }
}
