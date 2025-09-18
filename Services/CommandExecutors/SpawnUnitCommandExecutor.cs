﻿using Arch.Core;
using DVG.Core;
using DVG.SkyPirates.Shared.Commands;
using DVG.SkyPirates.Shared.Components;
using DVG.SkyPirates.Shared.Components.Data;
using DVG.SkyPirates.Shared.Entities;
using DVG.SkyPirates.Shared.IFactories;
using DVG.SkyPirates.Shared.IServices;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Services.CommandExecutors
{
    public class SpawnUnitCommandExecutor :
        ICommandExecutor<SpawnUnitCommand>
    {
        private readonly IUnitFactory _unitFactory;
        private readonly World _world;

        public SpawnUnitCommandExecutor(World world, IUnitFactory unitFactory)
        {
            _unitFactory = unitFactory;
            _world = world;
        }

        public void Execute(Command<SpawnUnitCommand> cmd)
        {
            var squad = EntityIds.Get(cmd.Data.SquadId);
            var pos = _world.Get<Position>(squad).Value;
            var unit = _unitFactory.Create((pos, cmd.ClientId, 1, cmd.Data.UnitId, cmd.EntityId));

            AddUnit(squad, unit);
        }

        private void AddUnit(Entity squad, Entity unit)
        {
            ref var squadComponent = ref _world.Get<Squad>(squad);
            squadComponent.units = new List<Entity>(squadComponent.units);
            squadComponent.units.Add(unit);
        }
    }
}
