using Delta.Netcode;
using DVG.SkyPirates.Shared.IServices;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;

namespace DVG.SkyPirates.Shared.Services.CommandValidators
{
    public sealed class FutureCommandValidator<T> : ICommandValidator<T>, ICommandValidatorRegistration
    {
        private readonly ITickCounterService _tickCounter;

        public FutureCommandValidator(ITickCounterService tickCounter)
        {
            _tickCounter = tickCounter;
        }

        public bool Validate(in Command<T> command)
        {
            return (long)_tickCounter.TickCounter + 1 >= command.Header.Step;
        }
    }
}
