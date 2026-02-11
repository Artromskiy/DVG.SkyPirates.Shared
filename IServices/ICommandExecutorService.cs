using DVG.Commands;

namespace DVG.SkyPirates.Shared.IServices
{
    public interface ICommandExecutorService
    {
        void Execute(CommandCollection commands);
    }
}
