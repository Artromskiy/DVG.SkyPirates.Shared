using Delta.Netcode;

namespace DVG.SkyPirates.Shared.IServices
{
    public interface ICommandAcceptanceService
    {
        bool TryAccept<T>(in Command<T> request, out Command<T> accepted);
        void PrepareLocal<T>(in Command<T> command, out Command<T> prepared);
    }

    public interface ICommandValidatorRegistration
    {
    }

    public interface ICommandMutatorRegistration
    {
    }
}
