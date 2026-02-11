using Arch.Core;
using DVG.Commands;
using DVG.Components;
using DVG.SkyPirates.Shared.Commands;
using DVG.SkyPirates.Shared.Components.Runtime;
using DVG.SkyPirates.Shared.Ids;
using DVG.SkyPirates.Shared.IFactories;
using DVG.SkyPirates.Shared.IServices;
using System;

namespace DVG.SkyPirates.Shared.Services.CommandExecutors
{
    public class SpawnUnitCommandExecutor :
        ICommandExecutor<SpawnUnitCommand>
    {
        private readonly IEntityFactory _commandEntityFactory;
        private readonly IConfigedEntityFactory<UnitId> _unitFactory;
        private readonly World _world;

        public SpawnUnitCommandExecutor(World world, IEntityFactory commandEntityFactory, IConfigedEntityFactory<UnitId> unitFactory)
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
            _world.AddOrGet<SquadMember>(unit).SquadId = _world.Get<SyncId>(squad).Value;
        }

    }
}
