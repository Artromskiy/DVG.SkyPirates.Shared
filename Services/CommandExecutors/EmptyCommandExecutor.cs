using DVG.Commands;
using DVG.SkyPirates.Shared.IServices;

namespace DVG.SkyPirates.Shared.Services.CommandExecutors
{
    public class EmptyCommandExecutor : ICommandExecutor<ICommandData>
    {
        public void Execute(Command<ICommandData> cmd) { }
    }
}
