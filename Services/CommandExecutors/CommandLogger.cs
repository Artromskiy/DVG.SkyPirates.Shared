using DVG.Core;
using DVG.SkyPirates.Shared.Commands;
using DVG.SkyPirates.Shared.IServices;
using System;

namespace DVG.SkyPirates.Shared.Services.CommandExecutors
{
    public class CommandLogger :
        ICommandExecutor<Direction>,
        ICommandExecutor<Rotation>,
        ICommandExecutor<Fixation>,
        ICommandExecutor<SpawnUnit>,
        ICommandExecutor<SpawnSquad>
    {
        public void Execute(Command<SpawnSquad> cmd) => ExecuteCommand(cmd);
        public void Execute(Command<SpawnUnit> cmd) => ExecuteCommand(cmd);
        public void Execute(Command<Fixation> cmd) => ExecuteCommand(cmd);
        public void Execute(Command<Rotation> cmd) => ExecuteCommand(cmd);
        public void Execute(Command<Direction> cmd) => Console.WriteLine($"{typeof(Direction)} {cmd.Data.direction}");

        private void ExecuteCommand<T>(Command<T> _)
            where T : ICommandData
        {
            Console.WriteLine(typeof(T));
        }
    }
}
