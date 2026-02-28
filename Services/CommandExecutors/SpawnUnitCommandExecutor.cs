using Arch.Core;
using DVG.Commands;
using DVG.Components;
using DVG.SkyPirates.Shared.Commands;
using DVG.SkyPirates.Shared.Components.Config;
using DVG.SkyPirates.Shared.Components.Runtime;
using DVG.SkyPirates.Shared.Data;
using DVG.SkyPirates.Shared.Ids;
using DVG.SkyPirates.Shared.IFactories;
using DVG.SkyPirates.Shared.IServices;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace DVG.SkyPirates.Shared.Services.CommandExecutors
{
    public class SpawnUnitCommandExecutor : ICommandExecutor<SpawnUnitCommand>
    {
        private readonly UnitsInfoConfig _unitsInfoConfig;
        private readonly IEntityRegistry _entityRegistryService;
        private readonly IConfigedEntityFactory<UnitId> _unitFactory;
        private readonly World _world;

        public SpawnUnitCommandExecutor(UnitsInfoConfig unitsInfoConfig, IEntityRegistry entityRegistryService, IConfigedEntityFactory<UnitId> unitFactory, World world)
        {
            _unitsInfoConfig = unitsInfoConfig;
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
                Console.WriteLine($"Attempt to use command for entity {cmd.Data.SquadId}, which is not created");
                return;
            }

            if (!TrySpawn(squad, cmd.Data.UnitId))
                return;

            var pos = _world.Get<Position>(squad);
            var unit = _unitFactory.Create((cmd.Data.UnitId, cmd.Data.CreationData));

            _world.Get<TeamId>(unit) = cmd.ClientId.Value;
            _world.Get<Position>(unit) = pos;
            _world.Get<GoodsDrop>(unit) = new() { Values = ImmutableSortedDictionary.Create<GoodsId, int>() };
            _world.AddOrGet<SquadMember>(unit).SquadId = _world.Get<SyncId>(squad).Value;
        }


        private bool TrySpawn(Entity squad, UnitId unitId)
        {
            // collect squad goods info
            // not enough => return
            // remove goods (first from squad then units sorted by syncId)
            // add new unit
            // end => redistribution system will do the work

            var squadId = _world.Get<SyncId>(squad);
            if (!_unitsInfoConfig.TryGetValue(unitId, out var info))
                return false;
            var price = info.RumPrice;
            ref var drop = ref _world.Get<GoodsDrop>(squad);
            var squadRum = drop.Values.GetValueOrDefault("Rum");
            if (squadRum >= price)
            {
                var newDrop = drop.Values.ToBuilder();
                newDrop["Rum"] -= price;
                drop = new() { Values = newDrop.ToImmutable() };
                return true;
            }

            var totalRum = squadRum;
            var desc = new QueryDescription().WithAll<SquadMember, GoodsDrop>();
            _world.Query(in desc, (ref SquadMember member, ref GoodsDrop drop) =>
            {
                if (member.SquadId == squadId)
                    totalRum += drop.Values.GetValueOrDefault("Rum");
            });

            if (totalRum < price)
                return false;

            List<(Entity entity, GoodsDrop drop, SyncId syncId)> units = new();
            _world.Query(desc, (Entity entity, ref SquadMember member, ref GoodsDrop drop, ref SyncId syncId) =>
            {
                if (member.SquadId == squadId)
                    units.Add((entity, drop, syncId));
            });

            int leftPrice = price;

            // remove from squad
            var squadNewDrop = drop.Values.ToBuilder();
            var removeSquad = Maths.Min(squadRum, price);
            if (squadRum > 0)
            {
                squadNewDrop["Rum"] -= removeSquad;
                leftPrice -= removeSquad;
                drop = new() { Values = squadNewDrop.ToImmutable() };
            }

            // remove from units
            foreach (var unit in units.OrderBy(u => u.syncId.Value))
            {
                int count = unit.drop.Values.GetValueOrDefault("Rum");
                var remove = Maths.Min(count, leftPrice);
                if (remove == 0)
                    continue;
                var newDrop = unit.drop.Values.ToBuilder();
                newDrop["Rum"] -= remove;
                leftPrice -= remove;
                _world.Get<GoodsDrop>(unit.entity) = new() { Values = newDrop.ToImmutable() };
                if (leftPrice == 0)
                    break;
            }
            return true;
        }
    }
}
