using DVG.Collections;
using DVG.Commands;
using DVG.SkyPirates.Shared.IServices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DVG.SkyPirates.Shared.Services
{
    public class CommandExecutorService : ICommandExecutorService
    {
        private readonly ICommandRecieveService _recieveService;

        private readonly GenericCollection _commands = new();
        private readonly (ICommandExecutor executor, Action<int> executorCall)[] _executors;

        public CommandExecutorService(ICommandRecieveService recieveService, IEnumerable<ICommandExecutor> executors)
        {
            _recieveService = recieveService;
            _executors = executors.Select(e =>
            {
                var action = new CommandCallAction(_recieveService, _commands, e);
                e.ForEach(ref action);
                return (e, action.wrappedCall);
            }).ToArray();
        }

        public void Tick(int tick, fix deltaTime)
        {
            foreach (var (_, executorCall) in _executors)
            {
                executorCall(tick);
            }
        }

        private struct CommandCallAction : IGenericAction
        {
            private readonly ICommandRecieveService _recieveService;
            private readonly GenericCollection _commands;
            private readonly ICommandExecutor _executor;
            public Action<int> wrappedCall;

            public CommandCallAction(ICommandRecieveService recieveService, GenericCollection commands, ICommandExecutor executor)
            {
                _recieveService = recieveService;
                _commands = commands;
                _executor = executor;
                wrappedCall = null!;
            }

            public void Invoke<T>()
            {
                var commands = _commands;
                _recieveService.RegisterReciever<T>(c =>
                {
                    if (!commands.TryGet<Dictionary<int, Command<T>>>(out var typedCommands))
                        commands.Add(typedCommands = new());
                    typedCommands.Add(c.Tick, c);
                });

                var executor = _executor as ICommandExecutor<T>;
                Debug.Assert(executor is not null);

                wrappedCall = (i) =>
                {
                    if (!commands.TryGet<Dictionary<int, Command<T>>>(out var typedCommands))
                        return;
                    if (!typedCommands.TryGetValue(i, out var command))
                        return;
                    executor.Execute(command);
                };
            }
        }
    }
}
