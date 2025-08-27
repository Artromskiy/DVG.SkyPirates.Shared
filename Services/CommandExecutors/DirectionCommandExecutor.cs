using Arch.Core.Extensions;
using DVG.Core;
using DVG.SkyPirates.Shared.Commands;
using DVG.SkyPirates.Shared.Components;
using DVG.SkyPirates.Shared.Components.Data;
using DVG.SkyPirates.Shared.Entities;
using DVG.SkyPirates.Shared.IServices;
using DVG.SkyPirates.Shared.Tools.Extensions;

namespace DVG.SkyPirates.Shared.Services.CommandExecutors
{
    public class DirectionCommandExecutor : ICommandExecutor<DirectionCommand>
    {
        public DirectionCommandExecutor()
        {

        }

        public void Execute(Command<DirectionCommand> cmd)
        {
            var squad = EntityIds.Get(cmd.EntityId);
            SetDirection(
                ref squad.Get<Direction>().Value,
                ref squad.Get<Rotation>().Value,
                cmd.Data.Direction);
        }

        public void SetDirection(ref fix2 direction, ref fix rotation, fix2 targetDirection)
        {
            direction = targetDirection;
            if (fix2.SqrLength(direction) == 0)
                return;

            var rotationRadians = MathsExtensions.GetRotation(direction);
            rotation = Maths.Degrees(rotationRadians);
        }
    }
}
