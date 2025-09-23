using Arch.Core;
using DVG.Core;
using DVG.SkyPirates.Shared.Commands;
using DVG.SkyPirates.Shared.Components;
using DVG.SkyPirates.Shared.Components.Data;
using DVG.SkyPirates.Shared.Entities;
using DVG.SkyPirates.Shared.IServices;
using DVG.SkyPirates.Shared.Tools.Extensions;
using System.Diagnostics;

namespace DVG.SkyPirates.Shared.Services.CommandExecutors
{
    public class DirectionCommandExecutor : ICommandExecutor<DirectionCommand>
    {
        private readonly World _world;
        public DirectionCommandExecutor(World world)
        {
            _world = world;
        }

        public void Execute(Command<DirectionCommand> cmd)
        {
            var squad = EntityIds.Get(cmd.EntityId);
            if (!_world.IsAlive(squad) ||
                !_world.Has<Direction>(squad) ||
                !_world.Has<Rotation>(squad))
            {
                Debug.WriteLine($"Attempt to use command for entity {cmd.EntityId}, which is not created");
                return;
            }

            SetDirection(ref _world.Get<Direction>(squad), ref _world.Get<Rotation>(squad), cmd.Data.Direction);
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
