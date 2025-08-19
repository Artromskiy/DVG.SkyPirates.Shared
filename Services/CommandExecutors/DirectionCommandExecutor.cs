using Arch.Core.Extensions;
using DVG.Core;
using DVG.SkyPirates.Shared.Commands;
using DVG.SkyPirates.Shared.Components;
using DVG.SkyPirates.Shared.Configs;
using DVG.SkyPirates.Shared.Entities;
using DVG.SkyPirates.Shared.IServices;
using System;

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
                ref squad.Get<Direction>().direction,
                ref squad.Get<Rotation>().rotation,
                ref squad.Get<Squad>(),
                cmd.Data.Direction);
        }

        public void SetDirection(ref fix2 _direction, ref fix _rotation, ref Squad squad, fix2 direction)
        {
            _direction = direction;

            if (fix2.SqrLength(_direction) == 0)
                return;
            var oldRot = _rotation;
            _rotation = GetRotation(_direction);

            static int Quantize(fix a) => (int)Maths.Round(Maths.Degrees(a) * 16 / 360);
            int newQuantizedRotation = Quantize(_rotation);
            int oldQuantizedRotation = Quantize(oldRot);
            int deltaRotation = (newQuantizedRotation - oldQuantizedRotation) & 15;
            if (deltaRotation == 0)
                return;
            var _packedCircles = GetCirclesConfig(squad._orders.Count);
            for (int i = 0; i < squad._orders.Count; i++)
                squad._orders[i] = _packedCircles.Reorders[deltaRotation, squad._orders[i]];
            UpdateRotatedPoints(ref squad, _rotation, _packedCircles);
        }

        private static fix GetRotation(fix2 direction)
        {
            return -Maths.Atan2(-direction.x, direction.y);
        }

        private void UpdateRotatedPoints(ref Squad squad, fix rotation, PackedCirclesConfig packedCircles)
        {
            Array.Resize(ref squad._rotatedPoints, packedCircles.Points.Length);
            for (int i = 0; i < packedCircles.Points.Length; i++)
            {
                var localPoint = packedCircles.Points[i] / 2;
                squad._rotatedPoints[i] = RotatePoint(localPoint, rotation);
            }
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
