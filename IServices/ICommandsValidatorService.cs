using DVG.Core;

namespace DVG.SkyPirates.Shared.IServices
{
    public interface ICommandsValidatorService
    {
        bool ValidateCommand<T>(Command<T> cmd) where T : ICommandData;
        bool ValidateClientId<T>(int clientId, Command<T> cmd) where T : ICommandData;
    }
}
