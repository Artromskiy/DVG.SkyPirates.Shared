using Arch.Core;
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
        private readonly World _world;
        public FixationCommandExecutor(World world)
        {
            _world = world;
        }

        public void Execute(Command<FixationCommand> cmd)
        {
            _world.Get<Fixation>(EntityIds.Get(cmd.EntityId)).Value = cmd.Data.Fixation;
        }
    }
}
