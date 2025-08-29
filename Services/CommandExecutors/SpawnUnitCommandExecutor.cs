using Arch.Core;
using Arch.Core.Extensions;
using DVG.Core;
using DVG.SkyPirates.Shared.Commands;
using DVG.SkyPirates.Shared.Components;
using DVG.SkyPirates.Shared.Components.Data;
using DVG.SkyPirates.Shared.Configs;
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

        public SpawnUnitCommandExecutor(IUnitFactory unitFactory)
        {
            _unitFactory = unitFactory;
        }

        public void Execute(Command<SpawnUnitCommand> cmd)
        {
            var squad = EntityIds.Get(cmd.Data.SquadId);
            var pos = squad.Get<Position>().Value;
            var unit = _unitFactory.Create((pos, cmd.ClientId, 1, cmd.Data.UnitId, cmd.EntityId));

            AddUnit(squad, unit);
        }

        private void AddUnit(Entity squad, Entity unit)
        {
            ref var squadComponent = ref squad.Get<Squad>();
            squadComponent.units = new List<Entity>(squadComponent.units);
            squadComponent.units.Add(unit);
        }
    }
}
