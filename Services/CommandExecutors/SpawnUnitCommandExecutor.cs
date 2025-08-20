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
using System.Collections.Generic;

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

            unit.Get<Position>().Value = squad.Get<Position>().Value;

            AddUnit(squad, unit);
        }

        void AddUnit(Entity squad, Entity unit)
        {
            ref var squadComponent = ref squad.Get<Squad>();
            var rotation = squad.Get<Rotation>().Value;
            int oldCount = squadComponent.units.Count;
            int newCount = oldCount + 1;
            var packedCircles = GetCirclesConfig(newCount);
            squadComponent.orders = new List<int>(squadComponent.orders);
            squadComponent.units = new List<Entity>(squadComponent.units);
            squadComponent.orders.Add(oldCount);
            squadComponent.units.Add(unit);
            squadComponent.positions = new fix2[newCount];

            for (int i = 0; i < newCount; i++)
            {
                var localPoint = packedCircles.Points[i] / 2;
                squadComponent.positions[i] = RotatePoint(localPoint, rotation);
            }
        }

        private PackedCirclesConfig GetCirclesConfig(int count)
        {
            return _circlesModelFactory.Create("Configs/PackedCircles/PackedCirclesModel" + count);
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
