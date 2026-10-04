using DVG.Collections;
using Delta.Netcode;
using DVG.SkyPirates.Shared.Commands;
using DVG.SkyPirates.Shared.IServices;
using System.Collections.Generic;
using System.Linq;
using CommandsRegistry = DVG.Commands.CommandsRegistry;
using IGenericAction = DVG.IGenericAction;

namespace DVG.SkyPirates.Shared.Services
{
    public class CommandExecutorService : ICommandExecutorService
    {
        private readonly ICommandReciever _commandReciever;

        private readonly GenericCollection _commands = new();
        private readonly ICommandExecutorRegistration[] _executors;

        public CommandExecutorService(ICommandReciever commandReciever, IEnumerable<ICommandExecutorRegistration> executors)
        {
            _commandReciever = commandReciever;
            _executors = executors.ToArray();

            _commandReciever.RegisterReciever<InvalidateCommand>(Invalidate);
            var registerAction = new RegisterRecieverAction(_commandReciever, _commands, _executors);
            CommandsRegistry.ForEach(ref registerAction);
        }

        public Dictionary<int, List<Command<T>>> GetCommands<T>()
        {
            _commands.TryGet<Dictionary<int, List<Command<T>>>>(out var typedCommands);
            return typedCommands;
        }

        public CommandsData GetCommands()
        {
            CommandsData commandsData = new();
            var packCommandAction = new PackCommand(commandsData, _commands);
            CommandsRegistry.ForEach(ref packCommandAction);

            return commandsData;
        }

        public void Tick(int tick)
        {
            var action = new ExecuteCommandAction(_commands, _executors, tick);
            CommandsRegistry.ForEach(ref action);
        }

        private void Invalidate(Command<InvalidateCommand> invalid)
        {
            var invalidateAction = new InvalidateAction(_commands, SkyPiratesCommand.GetTick(invalid), SkyPiratesCommand.GetClientId(invalid));
            CommandsRegistry.Call(invalid.Payload.CommandId, ref invalidateAction);
        }

        private readonly struct ExecuteCommandAction : IGenericAction
        {
            private readonly GenericCollection _commands;
            private readonly ICommandExecutorRegistration[] _executors;
            private readonly int _tick;

            public ExecuteCommandAction(GenericCollection commands, ICommandExecutorRegistration[] executors, int tick)
            {
                _commands = commands;
                _executors = executors;
                _tick = tick;
            }

            public void Invoke<T>()
            {
                if (!_commands.TryGet<Dictionary<int, List<Command<T>>>>(out var typedCommands))
                {
                    return;
                }

                if (!typedCommands.TryGetValue(_tick, out var tickCommands))
                {
                    return;
                }

                foreach (var registration in _executors)
                {
                    if (registration is not ICommandExecutor<T> executor)
                        continue;

                    foreach (var cmd in tickCommands)
                        executor.Execute(in cmd);
                }
            }
        }

        private readonly struct InvalidateAction : IGenericAction
        {
            private readonly GenericCollection _commands;
            private readonly int _tick;
            private readonly int _clientId;

            public InvalidateAction(GenericCollection commands, int tick, int clientId)
            {
                _commands = commands;
                _tick = tick;
                _clientId = clientId;
            }

            public void Invoke<T>()
            {
                if (!_commands.TryGet<Dictionary<int, List<Command<T>>>>(out var typedCommands))
                {
                    return;
                }

                if (!typedCommands.TryGetValue(_tick, out var tickCommands))
                {
                    return;
                }

                int match = tickCommands.FindIndex(Match);
                tickCommands.RemoveAt(match);
            }
            private bool Match<T>(Command<T> c) => SkyPiratesCommand.GetClientId(c) == _clientId;
        }

        private readonly struct RegisterRecieverAction : IGenericAction
        {
            private readonly ICommandReciever _commandReciever;
            private readonly GenericCollection _commands;
            private readonly ICommandExecutorRegistration[] _executors;

            public RegisterRecieverAction(ICommandReciever commandReciever, GenericCollection commands, ICommandExecutorRegistration[] executors)
            {
                _commandReciever = commandReciever;
                _commands = commands;
                _executors = executors;
            }

            public readonly void Invoke<T>()
            {
                bool hasExecutor = false;
                foreach (var executor in _executors)
                {
                    if (executor is ICommandExecutor<T>)
                    {
                        hasExecutor = true;
                        break;
                    }
                }

                if (!hasExecutor)
                    return;

                var commands = _commands;
                _commandReciever.RegisterReciever<T>(Recieve);
            }

            private readonly void Recieve<T>(Command<T> command)
            {
                if (!_commands.TryGet<Dictionary<int, List<Command<T>>>>(out var typedCommands))
                {
                    _commands.Add(typedCommands = new());
                }

                if (!typedCommands.TryGetValue(SkyPiratesCommand.GetTick(command), out var tickCommands))
                {
                    typedCommands[SkyPiratesCommand.GetTick(command)] = tickCommands = new();
                }

                tickCommands.Add(command);
            }
        }

        private readonly struct PackCommand : IGenericAction
        {
            private readonly CommandsData _commandsData;
            private readonly GenericCollection _commands;

            public PackCommand(CommandsData commandsData, GenericCollection commands)
            {
                _commandsData = commandsData;
                _commands = commands;
            }

            public readonly void Invoke<T>()
            {
                if (_commands.TryGet<Dictionary<int, List<Command<T>>>>(out var commands))
                {
                    _commandsData.Set(commands);
                }
            }
        }
    }
}
