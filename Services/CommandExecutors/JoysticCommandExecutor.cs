using Arch.Core;
using DVG.Core;
using DVG.SkyPirates.Shared.Commands;
using DVG.SkyPirates.Shared.Components;
using DVG.SkyPirates.Shared.Components.Runtime;
using DVG.SkyPirates.Shared.IFactories;
using DVG.SkyPirates.Shared.IServices;
using DVG.SkyPirates.Shared.Tools.Extensions;
using System;

namespace DVG.SkyPirates.Shared.Services.CommandExecutors
{
    public class JoysticCommandExecutor : ICommandExecutor<JoystickCommand>
    {
        private readonly IEntityFactory _commandEntityFactory;
        private readonly World _world;
        public JoysticCommandExecutor(World world, IEntityFactory commandEntityFactory)
        {
            _world = world;
            _commandEntityFactory = commandEntityFactory;
        }

        public void Execute(Command<JoystickCommand> cmd)
        {
            var entity = _commandEntityFactory.Get(cmd.EntityId);
            if (entity == Entity.Null ||
                !_world.IsAlive(entity) ||
                !_world.Has<Direction, Direction, Fixation>(entity))
            {
                Console.WriteLine($"Attempt to use command for entity {cmd.EntityId}, which is not created");
                return;
            }

            SetDirection(ref _world.Get<Direction>(entity), ref _world.Get<Rotation>(entity), cmd.Data.Direction);

            _world.Get<Fixation>(entity).Value = cmd.Data.Fixation;
        }

        public void SetDirection(ref Direction direction, ref Rotation rotation, fix2 targetDirection)
        {
            direction.Value = targetDirection;
            if (fix2.SqrLength(direction.Value) == 0)
                return;

            var rotationRadians = MathsExtensions.GetRotation(direction.Value);
            rotation.Value = Maths.Degrees(rotationRadians);
        }
    }
}
