using Arch.Core;
using Arch.Core.Extensions;
using DVG.Core;
using DVG.SkyPirates.Shared.Commands;
using DVG.SkyPirates.Shared.Components;
using DVG.SkyPirates.Shared.Configs;
using DVG.SkyPirates.Shared.Entities;
using DVG.SkyPirates.Shared.IFactories;
using DVG.SkyPirates.Shared.IServices;
using System;

namespace DVG.SkyPirates.Shared.Services.CommandExecutors
{
    public class SpawnUnitCommandExecutor :
        ICommandExecutor<SpawnUnitCommand>
    {
        private readonly IPathFactory<PackedCirclesConfig> _circlesModelFactory;
        private readonly IUnitFactory _unitFactory;

        public SpawnUnitCommandExecutor(IUnitFactory unitFactory, IPathFactory<PackedCirclesConfig> circlesModelFactory)
        {
            _circlesModelFactory = circlesModelFactory;
            _unitFactory = unitFactory;
        }

        public void Execute(Command<SpawnUnitCommand> cmd)
        {
            var squad = EntityIds.Get(cmd.Data.SquadId);
            var unit = _unitFactory.Create(cmd);

            unit.Get<Position>().position = squad.Get<Position>().position;

            AddUnit(squad, unit);
        }

        void AddUnit(Entity squad, Entity unit)
        {
            ref var squadComponent = ref squad.Get<Squad>();
            var packedCircles = GetCirclesConfig(squadComponent.units.Count + 1);
            squadComponent.orders.Add(squadComponent.units.Count);
            squadComponent.units.Add(unit);
            UpdateRotatedPoints(ref squadComponent, ref squad.Get<Rotation>(), packedCircles);
        }

        private PackedCirclesConfig GetCirclesConfig(int count)
        {
            return _circlesModelFactory.Create("Configs/PackedCircles/PackedCirclesModel" + count);
        }

        private void UpdateRotatedPoints(ref Squad squad, ref Rotation r, PackedCirclesConfig packedCircles)
        {
            Array.Resize(ref squad.positions, packedCircles.Points.Length);
            for (int i = 0; i < packedCircles.Points.Length; i++)
            {
                var localPoint = packedCircles.Points[i] / 2;
                squad.positions[i] = RotatePoint(localPoint, r.rotation);
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
    }
}
