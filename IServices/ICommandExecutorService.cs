using DVG.Core;

namespace DVG.SkyPirates.Shared.IServices
{
    public interface ICommandExecutorService
    {
        void Execute<T>(Command<T> cmd) where T : ICommandData;
        void Execute(CommandCollection commands);
    }
}
