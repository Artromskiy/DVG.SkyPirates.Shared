using DVG.Commands;
using DVG.SkyPirates.Shared.IServices;

namespace DVG.SkyPirates.Shared.Services.CommandExecutors
{
    public class EmptyCommandExecutor : ICommandExecutor
    {
        public void Execute<T>(Command<T> cmd) { }
    }
}
