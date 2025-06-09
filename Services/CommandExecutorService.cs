using DVG.Core;
using DVG.SkyPirates.Shared.IServices;
using System;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Services
{
    public class CommandExecutorService : ICommandExecutorService
    {
        private readonly IEnumerable<ICommandExecutor> _executors;

        public CommandExecutorService(IEnumerable<ICommandExecutor> executors)
        {
            _executors = executors;
        }

        public void Execute<T>(Command<T> cmd) where T : ICommandData
        {
            Console.WriteLine(cmd.ClientId);
            foreach (var executor in _executors)
                if(executor is ICommandExecutor<T> concreteExecutor)
                    concreteExecutor?.Execute(cmd);
        }
    }
}
