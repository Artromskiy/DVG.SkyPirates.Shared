using Arch.Core;
using DVG.Core;
using DVG.SkyPirates.Shared.Commands;
using DVG.SkyPirates.Shared.Components;
using DVG.SkyPirates.Shared.Components.Runtime;
using DVG.SkyPirates.Shared.Entities;
using DVG.SkyPirates.Shared.IServices;
using DVG.SkyPirates.Shared.Tools.Extensions;
using System;

namespace DVG.SkyPirates.Shared.Services.CommandExecutors
{
    public class JoysticCommandExecutor : ICommandExecutor<JoystickCommand>
    {
        private readonly World _world;
        public JoysticCommandExecutor(World world)
        {
            _world = world;
        }

        public void Execute(Command<JoystickCommand> cmd)
        {
            var squad = EntityIds.Get(cmd.EntityId);
            if (!_world.IsAlive(squad) ||
                !_world.Has<Direction>(squad) ||
                !_world.Has<Rotation>(squad) ||
                !_world.Has<Fixation>(squad))
            {
                Console.WriteLine($"Attempt to use command for entity {cmd.EntityId}, which is not created");
                return;
            }

            SetDirection(ref _world.Get<Direction>(squad), ref _world.Get<Rotation>(squad), cmd.Data.Direction);

            _world.Get<Fixation>(squad).Value = cmd.Data.Fixation;
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
