using DVG.Core;
using DVG.SkyPirates.Shared.IServices;
using System.Collections.Generic;
using System.Linq;

namespace DVG.SkyPirates.Shared.Services
{
    public class CommandExecutorService : ICommandExecutorService
    {
        private readonly ICommandExecutor[] _executors;

        public CommandExecutorService(IEnumerable<ICommandExecutor> executors)
        {
            _executors = executors.ToArray();
        }

        public void Execute<T>(Command<T> cmd) where T : ICommandData
        {
            foreach (var executor in _executors)
                if(executor is ICommandExecutor<T> concreteExecutor)
                    concreteExecutor?.Execute(cmd);
        }
    }
}
