using Arch.Core.Extensions;
using DVG.Core;
using DVG.SkyPirates.Shared.Commands;
using DVG.SkyPirates.Shared.Components;
using DVG.SkyPirates.Shared.Configs;
using DVG.SkyPirates.Shared.Entities;
using DVG.SkyPirates.Shared.IServices;
using System;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Services.CommandExecutors
{
    public class DirectionCommandExecutor : ICommandExecutor<DirectionCommand>
    {
        private readonly IPathFactory<PackedCirclesConfig> _circlesModelFactory;

        public DirectionCommandExecutor(IPathFactory<PackedCirclesConfig> circlesModelFactory)
        {
            _circlesModelFactory = circlesModelFactory;
        }

        public void Execute(Command<DirectionCommand> cmd)
        {
            var squad = EntityIds.Get(cmd.EntityId);
            SetDirection(
                ref squad.Get<Direction>().Value,
                ref squad.Get<Rotation>().Value,
                ref squad.Get<Squad>(),
                cmd.Data.Direction);
        }

        public void SetDirection(ref fix2 direction, ref fix rotation, ref Squad squad, fix2 targetDirection)
        {
            direction = targetDirection;
            int count = squad.positions.Length;
            if (fix2.SqrLength(direction) == 0 || count == 0)
                return;

            var oldRot = rotation;
            rotation = GetRotation(direction);

            static int Quantize(fix a) => (int)Maths.Round(Maths.Degrees(a) * 16 / 360);
            int newQuantizedRotation = Quantize(rotation);
            int oldQuantizedRotation = Quantize(oldRot);
            int deltaRotation = (newQuantizedRotation - oldQuantizedRotation) & 15;
            if (deltaRotation == 0)
                return;

            var packedCircles = GetCirclesConfig(count);
            var oldOrders = squad.orders;

            squad.orders = new List<int>();
            for (int i = 0; i < count; i++)
                squad.orders.Add(packedCircles.Reorders[deltaRotation, oldOrders[i]]);

            squad.positions = new fix2[count];
            for (int i = 0; i < count; i++)
            {
                var localPoint = packedCircles.Points[i] / 2;
                squad.positions[i] = RotatePoint(localPoint, rotation);
            }
        }

        private static fix GetRotation(fix2 direction)
        {
            return -Maths.Atan2(-direction.x, direction.y);
        }

        public static fix2 RotatePoint(fix2 vec, fix radians)
        {
            var cs = Maths.Cos(radians);
            var sn = Maths.Sin(radians);
            fix x = vec.x * cs + vec.y * sn;
            fix y = -vec.x * sn + vec.y * cs;
            return new fix2(x, y);
        }

        private PackedCirclesConfig GetCirclesConfig(int count)
        {
            return _circlesModelFactory.Create("Configs/PackedCircles/PackedCirclesModel" + count);
        }
    }
}
