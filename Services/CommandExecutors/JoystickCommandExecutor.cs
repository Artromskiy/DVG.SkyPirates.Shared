using Arch.Core;
using DVG.Commands;
using DVG.Components;
using DVG.SkyPirates.Shared.Commands;
using DVG.SkyPirates.Shared.Components.Runtime;
using DVG.SkyPirates.Shared.IServices;
using DVG.SkyPirates.Shared.Tools.Extensions;
using System;

namespace DVG.SkyPirates.Shared.Services.CommandExecutors
{
    [Obsolete("Should handle if squad is empty")]
    public class JoystickCommandExecutor : ICommandExecutor<JoystickCommand>
    {
        private readonly IEntityRegistry _entityRegistryService;
        private readonly World _world;

        public JoystickCommandExecutor(IEntityRegistry entityRegistryService, World world)
        {
            _entityRegistryService = entityRegistryService;
            _world = world;
        }

        public void Execute(Command<JoystickCommand> cmd)
        {
            _entityRegistryService.TryGet(cmd.Data.Target, out var entity);

            if (entity == Entity.Null ||
                !_world.IsAlive(entity) ||
                !_world.Has<Alive>(entity))
            {
                Console.WriteLine($"Attempt to use command for entity {cmd.Data.Target}, which is not created");
                return;
            }

            ref var dir = ref _world.Get<Direction>(entity);
            ref var rot = ref _world.Get<Rotation>(entity);
            ref var fix = ref _world.Get<Fixation>(entity);
            dir = cmd.Data.Direction;
            fix = cmd.Data.Fixation;

            if (fix2.SqrLength(dir) == 0)
                return;
            rot = Maths.Degrees(MathsExtensions.GetRotation(dir));
        }
    }
}
