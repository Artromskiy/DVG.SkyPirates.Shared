using Arch.Core;
using DVG;
using DVG.SkyPirates.Shared.Components;
using DVG.SkyPirates.Shared.Components.Config;
using DVG.SkyPirates.Shared.Components.Framed;
using DVG.SkyPirates.Shared.Components.Runtime;
using DVG.SkyPirates.Shared.Components.Special;
using DVG.SkyPirates.Shared.IFactories;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;
using System.Collections.Generic;

public sealed class SquadUnitsSystem : ITickableExecutor
{
    private readonly QueryDescription _unitsDesc = new QueryDescription().
        WithAll<SquadMember, Destination>();

    private readonly QueryDescription _squadsDesc = new QueryDescription().
        WithAll<Squad, SyncId, Position, Rotation, Fixation, TargetSearchDistance, TargetSearchPosition>();

    private readonly World _world;
    private readonly IPackedCirclesFactory _packedCirclesFactory;

    private readonly Dictionary<int, List<(Entity entity, SyncId syncId)>> _unitsBySquad = new();

    public SquadUnitsSystem(World world, IPackedCirclesFactory packedCirclesFactory)
    {
        _world = world;
        _packedCirclesFactory = packedCirclesFactory;
    }

    public void Tick(int tick, fix deltaTime)
    {
        _unitsBySquad.Clear();

        var collectQuery = new CollectUnitsQuery(_unitsBySquad);
        _world.InlineEntityQuery<CollectUnitsQuery, SquadMember, SyncId>(_unitsDesc, ref collectQuery);

        foreach (var item in _unitsBySquad)
        {
            item.Value.Sort((u1, u2) => u1.syncId.Value.CompareTo(u2.syncId.Value));
        }

        var applyQuery = new ApplyFormationQuery(_world, _unitsBySquad, _packedCirclesFactory);
        _world.InlineQuery<ApplyFormationQuery,
            Squad, SyncId, Position, Rotation, TargetSearchDistance, TargetSearchPosition>
            (_squadsDesc, ref applyQuery);
    }

    private readonly struct CollectUnitsQuery : IForEachWithEntity<SquadMember, SyncId>
    {
        private readonly Dictionary<int, List<(Entity, SyncId)>> _map;

        public CollectUnitsQuery(Dictionary<int, List<(Entity, SyncId)>> map)
        {
            _map = map;
        }

        public void Update(Entity entity, ref SquadMember member, ref SyncId syncId)
        {
            if (!_map.TryGetValue(member.SquadId, out var list))
                _map[member.SquadId] = list = new(8);

            list.Add((entity, syncId));
        }
    }

    private readonly struct ApplyFormationQuery
        : IForEach<Squad, SyncId, Position, Rotation, TargetSearchDistance, TargetSearchPosition>
    {
        private readonly World _world;
        private readonly Dictionary<int, List<(Entity entity, SyncId syncId)>> _unitsBySquad;
        private readonly IPackedCirclesFactory _factory;

        public ApplyFormationQuery(
            World world,
            Dictionary<int, List<(Entity, SyncId)>> unitsBySquad,
            IPackedCirclesFactory factory)
        {
            _world = world;
            _unitsBySquad = unitsBySquad;
            _factory = factory;
        }

        public void Update(
            ref Squad squad,
            ref SyncId syncId,
            ref Position pos,
            ref Rotation rot,
            ref TargetSearchDistance searchDist,
            ref TargetSearchPosition searchPos)
        {
            if (!_unitsBySquad.TryGetValue(syncId.Value, out var units) || units.Count == 0)
                return;

            var circles = _factory.Create(units.Count);

            for (int i = 0; i < units.Count; i++)
            {
                var unit = units[i];
                ref var destination = ref _world.Get<Destination>(unit.entity);
                var local = circles.Points[i] / 2;
                destination.Position = pos.Value + local.x_y;
                destination.Rotation = rot.Value;

                _world.Get<TargetSearchPosition>(unit.entity) = searchPos;
                _world.Get<TargetSearchDistance>(unit.entity) = searchDist;
            }
        }
    }
}
