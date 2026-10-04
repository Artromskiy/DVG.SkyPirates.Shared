using Delta.Netcode;
using DVG.Core;
using DVG.SkyPirates.Shared.Commands;
using DVG.SkyPirates.Shared.IServices;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;

namespace DVG.SkyPirates.Shared.Services.CommandValidators
{
    public sealed class LateCommandValidator<T> : ICommandValidator<T>, ICommandValidatorRegistration
    {
        private readonly ITickCounterService _tickCounter;

        public LateCommandValidator(ITickCounterService tickCounter)
        {
            _tickCounter = tickCounter;
        }

        public bool Validate(in Command<T> command)
        {
            return SkyPiratesCommand.GetTick(command) > _tickCounter.TickCounter - Constants.ValidTicksCount;
        }
    }
}
