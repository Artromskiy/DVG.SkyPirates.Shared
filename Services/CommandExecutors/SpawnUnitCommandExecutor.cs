using Arch.Core;
using DVG.Core;
using DVG.SkyPirates.Shared.Commands;
using DVG.SkyPirates.Shared.Components;
using DVG.SkyPirates.Shared.Components.Runtime;
using DVG.SkyPirates.Shared.IFactories;
using DVG.SkyPirates.Shared.IServices;
using System;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Services.CommandExecutors
{
    public class SpawnUnitCommandExecutor :
        ICommandExecutor<SpawnUnitCommand>
    {
        private readonly ICommandEntityFactory _commandEntityFactory;
        private readonly IUnitFactory _unitFactory;
        private readonly World _world;

        public SpawnUnitCommandExecutor(World world, IUnitFactory unitFactory, ICommandEntityFactory commandEntityFactory)
        {
            _world = world;
            _unitFactory = unitFactory;
            _commandEntityFactory = commandEntityFactory;
        }

        public void Execute(Command<SpawnUnitCommand> cmd)
        {
            var squad = _commandEntityFactory.Get(cmd.Data.SquadId);
            if (squad == Entity.Null ||
                !_world.IsAlive(squad) ||
                !_world.Has<Position, Squad>(squad))
            {
                Console.WriteLine($"Attempt to use command for entity {cmd.EntityId}, which is not created");
                return;
            }

            var pos = _world.Get<Position>(squad).Value;
            var unit = _unitFactory.Create((cmd.Data.UnitId, cmd.EntityId));

            _world.Get<Team>(unit).Id = cmd.ClientId;
            _world.Get<Position>(unit).Value = pos;

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
