using Arch.Core;
using DVG.Core;
using DVG.SkyPirates.Shared.Commands;
using DVG.SkyPirates.Shared.Components;
using DVG.SkyPirates.Shared.Entities;
using DVG.SkyPirates.Shared.IServices;
using System;

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
            var squad = EntityIds.Get(cmd.EntityId);
            if (!_world.IsAlive(squad) ||
                !_world.Has<Fixation>(squad))
            {
                Console.WriteLine($"Attempt to use command for entity {cmd.EntityId}, which is not created");
                return;
            }

            _world.Get<Fixation>(squad).Value = cmd.Data.Fixation;
        }
    }
}
