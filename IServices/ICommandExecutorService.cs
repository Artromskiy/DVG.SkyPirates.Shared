using DVG.Core;

namespace DVG.SkyPirates.Shared.IServices
{
    public interface ICommandExecutorService
    {
        void Execute(CommandCollection commands);
    }
}
