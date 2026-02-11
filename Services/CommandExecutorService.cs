using DVG.Commands;
using DVG.SkyPirates.Shared.IServices;
using System.Collections.Generic;
using System.Linq;

namespace DVG.SkyPirates.Shared.Services
{
    public class CommandExecutorService : ICommandExecutorService
    {
        private readonly ICommandExecutor[] _executors;
        private readonly ICommandExecutorWrapper[] _executorWrappers;

        public CommandExecutorService(IEnumerable<ICommandExecutor> executors)
        {
            _executors = executors.ToArray();
            List<ICommandExecutorWrapper> _wrappersList = new List<ICommandExecutorWrapper>();
            foreach (var item in _executors)
            {
                var action = new TryCreateWrapper(_wrappersList, item);
                CommandIds.ForEachData(ref action);
            }
            _executorWrappers = _wrappersList.ToArray();
        }

        public void Execute(CommandCollection commands)
        {
            foreach (var item in _executorWrappers)
            {
                item.Execute(commands);
            }
        }

        private interface ICommandExecutorWrapper
        {
            void Execute(CommandCollection commands);
        }

        private class CommandExecutorWrapper<T> : ICommandExecutorWrapper
            where T : ICommandData
        {
            private readonly ICommandExecutor<T> _executor;

            public CommandExecutorWrapper(ICommandExecutor<T> executor)
            {
                _executor = executor;
            }

            public void Execute(CommandCollection commands)
            {
                var genericCollection = commands.GetCollection<T>();
                if (genericCollection == null)
                    return;
                foreach (var cmd in genericCollection)
                {
                    _executor.Execute(cmd);
                }
            }
        }


        private struct TryCreateWrapper : IGenericAction<ICommandData>
        {
            public List<ICommandExecutorWrapper> _wrappers;
            public ICommandExecutor _executor;

            public TryCreateWrapper(List<ICommandExecutorWrapper> wrappers, ICommandExecutor executor) : this()
            {
                _wrappers = wrappers;
                _executor = executor;
            }

            public readonly void Invoke<T>() where T : ICommandData
            {
                if (_executor is ICommandExecutor<T> genericExecutor)
                    _wrappers.Add(new CommandExecutorWrapper<T>(genericExecutor));
            }
        }
    }
}
