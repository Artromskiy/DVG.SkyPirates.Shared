using DVG.Commands;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.IServices
{
    public interface ICommandExecutorService
    {
        void Tick(int tick, fix deltaTime);
        Dictionary<int, Command<T>> GetCommands<T>();
    }
}
