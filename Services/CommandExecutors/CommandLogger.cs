using DVG.Core;
using DVG.SkyPirates.Shared.Commands;
using DVG.SkyPirates.Shared.IServices;
using System;

namespace DVG.SkyPirates.Shared.Services.CommandExecutors
{
    public class CommandLogger :
        ICommandExecutor<Position>,
        ICommandExecutor<Rotation>,
        ICommandExecutor<Fixation>,
        ICommandExecutor<SpawnUnit>,
        ICommandExecutor<SpawnSquad>
    {
        public void Execute(Command<SpawnSquad> cmd) => ExecuteCommand(cmd);
        public void Execute(Command<SpawnUnit> cmd) => ExecuteCommand(cmd);
        public void Execute(Command<Fixation> cmd) => ExecuteCommand(cmd);
        public void Execute(Command<Rotation> cmd) => ExecuteCommand(cmd);
        public void Execute(Command<Position> cmd) => ExecuteCommand(cmd);

        private void ExecuteCommand<T>(Command<T> cmd)
            where T : ICommandData
        {
            Console.WriteLine(typeof(T));
        }
    }
}
