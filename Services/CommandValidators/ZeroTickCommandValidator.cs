using Delta.Netcode;
using DVG.SkyPirates.Shared.Commands;
using DVG.SkyPirates.Shared.IServices;

namespace DVG.SkyPirates.Shared.Services.CommandValidators
{
    public sealed class ZeroTickCommandValidator<T> : ICommandValidator<T>, ICommandValidatorRegistration
    {
        public bool Validate(in Command<T> command)
        {
            return SkyPiratesCommand.GetTick(command) > 0;
        }
    }
}
