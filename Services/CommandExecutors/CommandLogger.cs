using DVG.Core;
using DVG.SkyPirates.Shared.Commands;
using DVG.SkyPirates.Shared.IServices;
using System;

namespace DVG.SkyPirates.Shared.Services.CommandExecutors
{
    public class CommandLogger :
        ICommandExecutor<DirectionCommand>,
        ICommandExecutor<FixationCommand>,
        ICommandExecutor<SpawnUnitCommand>,
        ICommandExecutor<SpawnSquadCommand>
    {
        public void Execute(Command<SpawnSquadCommand> cmd) => ExecuteCommand(cmd);
        public void Execute(Command<SpawnUnitCommand> cmd) => ExecuteCommand(cmd);
        public void Execute(Command<FixationCommand> cmd) => ExecuteCommand(cmd);
        public void Execute(Command<DirectionCommand> cmd) => Console.WriteLine($"{typeof(DirectionCommand)} {cmd.Data.Direction}");

        private void ExecuteCommand<T>(Command<T> _)
            where T : ICommandData
        {
            Console.WriteLine(typeof(T));
        }
    }
}
