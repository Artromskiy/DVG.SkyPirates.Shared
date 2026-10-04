using Delta.Netcode;
using DVG.SkyPirates.Shared.IServices;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DVG.SkyPirates.Shared.Services
{
    public sealed class CommandAcceptanceService : ICommandAcceptanceService
    {
        private readonly object _sync = new();
        private readonly IEntityRegistry _entityRegistry;
        private readonly ICommandValidatorRegistration[] _validators;
        private readonly ICommandMutatorRegistration[] _mutators;
        private ulong _randomState;

        public CommandAcceptanceService(
            IEntityRegistry entityRegistry,
            IEnumerable<ICommandValidatorRegistration> validators,
            IEnumerable<ICommandMutatorRegistration> mutators)
        {
            _entityRegistry = entityRegistry;
            _validators = validators.ToArray();
            _mutators = mutators.ToArray();
            _randomState = (uint)new Random().Next();
        }

        public bool TryAccept<T>(in Command<T> request, out Command<T> accepted)
        {
            lock (_sync)
            {
                foreach (var registration in _validators)
                {
                    if (registration is ICommandValidator<T> validator && !validator.Validate(in request))
                    {
                        accepted = default;
                        return false;
                    }
                }

                accepted = Prepare(in request);
                return true;
            }
        }

        public void PrepareLocal<T>(in Command<T> command, out Command<T> prepared)
        {
            lock (_sync)
                prepared = Prepare(in command);
        }

        private Command<T> Prepare<T>(in Command<T> command)
        {
            var preparation = new CommandPreparation((ulong)_entityRegistry.NextId, _randomState);
            var payload = command.Payload;
            foreach (var registration in _mutators)
            {
                if (registration is ICommandMutator<T> mutator)
                    mutator.Mutate(ref payload, preparation);
            }

            _randomState = preparation.Capture().RandomState;
            return new Command<T>(command.Header, payload);
        }
    }
}
