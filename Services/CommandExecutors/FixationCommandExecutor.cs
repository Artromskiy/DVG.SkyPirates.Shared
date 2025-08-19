using Arch.Core.Extensions;
using DVG.Core;
using DVG.SkyPirates.Shared.Commands;
using DVG.SkyPirates.Shared.Components;
using DVG.SkyPirates.Shared.Entities;
using DVG.SkyPirates.Shared.IServices;

namespace DVG.SkyPirates.Shared.Services.CommandExecutors
{
    public class FixationCommandExecutor :
        ICommandExecutor<FixationCommand>
    {
        public FixationCommandExecutor() { }

        public void Execute(Command<FixationCommand> cmd)
        {
            EntityIds.Get(cmd.EntityId).Get<Fixation>().fixation = cmd.Data.Fixation;
        }
    }
}
