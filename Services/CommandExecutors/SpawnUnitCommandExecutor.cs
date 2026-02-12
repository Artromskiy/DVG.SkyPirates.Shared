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
        private readonly IEntityRegistryService _entityRegistryService;
        private readonly IConfigedEntityFactory<UnitId> _unitFactory;
        private readonly World _world;

        public SpawnUnitCommandExecutor(IEntityRegistryService entityRegistryService, IConfigedEntityFactory<UnitId> unitFactory, World world)
        {
            _entityRegistryService = entityRegistryService;
            _unitFactory = unitFactory;
            _world = world;
        }

        public void Execute(Command<SpawnUnitCommand> cmd)
        {
            _entityRegistryService.TryGet(new SyncId() { Value = cmd.Data.SquadId }, out var squad);
            if (squad == Entity.Null ||
                !_world.IsAlive(squad) ||
                !_world.Has<Position, Squad>(squad))
            {
                Console.WriteLine($"Attempt to use command for entity {cmd.EntityId}, which is not created");
                return;
            }

            var pos = _world.Get<Position>(squad);
            SyncId syncId = new() { Value = cmd.EntityId };
            var unit = _unitFactory.Create((cmd.Data.UnitId, new(syncId, default, default)));

            _world.Get<Team>(unit).Id = cmd.ClientId;
            _world.Get<Position>(unit) = pos;
            _world.AddOrGet<SquadMember>(unit).SquadId = _world.Get<SyncId>(squad).Value;
        }

    }
}
