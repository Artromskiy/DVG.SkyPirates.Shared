using DVG.Collections;
using DVG.Commands;
using DVG.SkyPirates.Shared.Commands;
using DVG.SkyPirates.Shared.IServices;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DVG.SkyPirates.Shared.Services
{
    public class CommandExecutorService : ICommandExecutorService
    {
        private readonly ICommandReciever _commandReciever;

        private readonly GenericCollection _commands = new();
        private readonly ICommandExecutor[] _executors;

        public CommandExecutorService(ICommandReciever commandReciever, IEnumerable<ICommandExecutor> executors)
        {
            _commandReciever = commandReciever;
            _executors = executors.ToArray();

            _commandReciever.RegisterReciever<InvalidateCommand>(Invalidate);
            foreach (ICommandExecutor executor in _executors)
            {
                var action = new RegisterRecieverAction(_commandReciever, _commands);
                executor.Call(ref action);
            }
        }

        public Dictionary<int, List<Command<T>>> GetCommands<T>()
        {
            _commands.TryGet<Dictionary<int, List<Command<T>>>>(out var typedCommands);
            return typedCommands;
        }

        public void Tick(int tick)
        {
            foreach (var executor in _executors)
            {
                var action = new ExecuteCommandAction(_commands, executor, tick);
                executor.Call(ref action);
            }
        }

        private void Invalidate(Command<InvalidateCommand> invalid)
        {
            var invalidateAction = new InvalidateAction(_commands, invalid.Tick, invalid.ClientId);
            CommandsRegistry.Call(invalid.Data.CommandId, ref invalidateAction);
        }

        private readonly struct ExecuteCommandAction : IGenericAction
        {
            private readonly GenericCollection _commands;
            private readonly ICommandExecutor _executor;
            private readonly int _tick;

            public ExecuteCommandAction(GenericCollection commands, ICommandExecutor executor, int tick)
            {
                _commands = commands;
                _executor = executor;
                _tick = tick;
            }

            public void Invoke<T>()
            {
                if (!_commands.TryGet<Dictionary<int, List<Command<T>>>>(out var typedCommands))
                    return;
                if (!typedCommands.TryGetValue(_tick, out var tickCommands))
                    return;

                var executor = _executor as ICommandExecutor<T>;
                Debug.Assert(executor is not null);

                foreach (var cmd in tickCommands)
                    executor.Execute(cmd);
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
                    return;
                if (!typedCommands.TryGetValue(_tick, out var tickCommands))
                    return;

                var match = tickCommands.FindIndex(Match);
                tickCommands.RemoveAt(match);
            }
            private bool Match<T>(Command<T> c) => c.ClientId == _clientId;
        }

        private readonly struct RegisterRecieverAction : IGenericAction
        {
            private readonly ICommandReciever _commandReciever;
            private readonly GenericCollection _commands;

            public RegisterRecieverAction(ICommandReciever commandReciever, GenericCollection commands)
            {
                _commandReciever = commandReciever;
                _commands = commands;
            }

            public readonly void Invoke<T>()
            {
                var commands = _commands;
                _commandReciever.RegisterReciever<T>(Recieve);
            }

            private readonly void Recieve<T>(Command<T> command)
            {
                if (!_commands.TryGet<Dictionary<int, List<Command<T>>>>(out var typedCommands))
                    _commands.Add(typedCommands = new());
                if (!typedCommands.TryGetValue(command.Tick, out var tickCommands))
                    typedCommands[command.Tick] = tickCommands = new();
                tickCommands.Add(command);
            }
        }
    }
}
