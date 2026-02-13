using Arch.Core;
using DVG.Commands;
using DVG.SkyPirates.Shared.Commands;
using DVG.SkyPirates.Shared.Components.Runtime;
using DVG.SkyPirates.Shared.IServices;
using DVG.SkyPirates.Shared.Tools.Extensions;
using System;

namespace DVG.SkyPirates.Shared.Services.CommandExecutors
{
    [Obsolete]
    public class JoysticCommandExecutor : ICommandExecutor<JoystickCommand>
    {
        private readonly IEntityRegistryService _entityRegistryService;
        private readonly World _world;

        public JoysticCommandExecutor(IEntityRegistryService entityRegistryService, World world)
        {
            _entityRegistryService = entityRegistryService;
            _world = world;
        }

        public void Execute(Command<JoystickCommand> cmd)
        {
            _entityRegistryService.TryGet(cmd.Data.Target, out var entity);

            if (entity == Entity.Null ||
                !_world.IsAlive(entity) ||
                !_world.Has<Direction, Fixation>(entity))
            {
                Console.WriteLine($"Attempt to use command for entity {cmd.Data.Target}, which is not created");
                return;
            }

            SetDirection(ref _world.Get<Direction>(entity), ref _world.Get<Rotation>(entity), cmd.Data.Direction);

            _world.Get<Fixation>(entity) = cmd.Data.Fixation;
        }

        public void SetDirection(ref Direction direction, ref Rotation rotation, fix2 targetDirection)
        {
            direction = targetDirection;
            if (fix2.SqrLength(direction) == 0)
                return;

            var rotationRadians = MathsExtensions.GetRotation(direction);
            rotation = Maths.Degrees(rotationRadians);
        }
    }
}
