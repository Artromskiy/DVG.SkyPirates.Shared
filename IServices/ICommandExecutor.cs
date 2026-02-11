using DVG.Commands;
using DVG.Core;

namespace DVG.SkyPirates.Shared.IServices
{
    public interface ICommandExecutor { }
    public interface ICommandExecutor<T> : ICommandExecutor
        where T : ICommandData
    {
        void Execute(Command<T> cmd);
    }
}
