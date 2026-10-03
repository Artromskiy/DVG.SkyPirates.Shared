using Delta;
using Delta.ECS;
using DVG.SkyPirates.Shared.Ecs;
using DVG.Components;
using DVG.Core.Collections;
using DVG.SkyPirates.Shared.Components.Framed;
using DVG.SkyPirates.Shared.Components.Runtime;
using DVG.SkyPirates.Shared.Data;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Systems
{
    public class SquadMemberDestinationSystem : IDeltaTickableExecutor
    {
        private Query? _unitsDescCache;
        private Query _unitsDesc => _unitsDescCache ??= _world.
            WhereAll<SquadMember, SyncId, Destination>().Alive().NotDisabled();

        private Query? _squadsDescCache;
        private Query _squadsDesc => _squadsDescCache ??= _world.
            WhereAll<Squad, SyncId, Position, Rotation, SquadMemberCount>().Alive().NotDisabled();

        private readonly World _world;
        private readonly PackedCirclesConfig _circlesConfig;

        private readonly Lookup<int> _orderPerUnit = new();
        private readonly Lookup<SquadData> _dataPerSquad = new();
        private readonly Dictionary<int, List<SyncId>> _unitsPerSquad = new();

        private readonly Queue<List<SyncId>> _unitsCache = new();

        public SquadMemberDestinationSystem(World world, PackedCirclesConfig circlesConfig)
        {
            _world = world;
            _circlesConfig = circlesConfig;
        }

        public void Tick(int tick, fix deltaTime)
        {
            foreach (var item in _unitsPerSquad)
            {
                item.Value.Clear();
                _unitsCache.Enqueue(item.Value);
            }
            _orderPerUnit.Clear();
            _dataPerSquad.Clear();
            _unitsPerSquad.Clear();

            var dataPerSquad = _dataPerSquad;
            var squadsDesc = _squadsDesc;
            _world.ForEach<Lookup<SquadData>, SyncId, Position, Rotation, SquadMemberCount>(in squadsDesc, ref dataPerSquad,
                static (ref Lookup<SquadData> data, ref SyncId syncId, ref Position position, ref Rotation rotation, ref SquadMemberCount memberCount) =>
                    data[syncId.Value] = new(position, rotation, memberCount));

            (Dictionary<int, List<SyncId>> UnitsPerSquad, Queue<List<SyncId>> UnitsCache) collectUnitsState = (_unitsPerSquad, _unitsCache);
            var unitsDesc = _unitsDesc;
            _world.ForEach<(Dictionary<int, List<SyncId>> UnitsPerSquad, Queue<List<SyncId>> UnitsCache), SquadMember, SyncId>(in unitsDesc, ref collectUnitsState,
                static (ref (Dictionary<int, List<SyncId>> UnitsPerSquad, Queue<List<SyncId>> UnitsCache) state, ref SquadMember member, ref SyncId syncId) =>
                {
                    if (!state.UnitsPerSquad.TryGetValue(member.SquadId, out var list))
                        state.UnitsPerSquad[member.SquadId] = state.UnitsCache.TryDequeue(out list) ? list : list = new(8); // really wtf?
                    list.Add(syncId);
                });

            foreach (var item in _unitsPerSquad)
            {
                item.Value.Sort((u1, u2) => u1.Value.CompareTo(u2.Value));
                for (var i = 0; i < item.Value.Count; i++)
                {
                    _orderPerUnit[item.Value[i].Value] = i;
                }
            }

            (Lookup<int> OrderPerUnit, Lookup<SquadData> DataPerSquad, PackedCirclesConfig CirclesConfig) applyState = (_orderPerUnit, _dataPerSquad, _circlesConfig);
            _world.ForEach<(Lookup<int> OrderPerUnit, Lookup<SquadData> DataPerSquad, PackedCirclesConfig CirclesConfig), SyncId, SquadMember, Destination>(in unitsDesc, ref applyState,
                static (ref (Lookup<int> OrderPerUnit, Lookup<SquadData> DataPerSquad, PackedCirclesConfig CirclesConfig) state, ref SyncId syncId, ref SquadMember member, ref Destination destination) =>
                {
                    var squad = state.DataPerSquad[member.SquadId];
                    var circles = state.CirclesConfig[squad.MemberCount];
                    var order = state.OrderPerUnit[syncId.Value];
                    var local = circles.Points[order];
                    destination.Position = squad.Position + local.x_y;
                    destination.Rotation = squad.Rotation;
                });
        }

        internal readonly struct SquadData
        {
            public readonly Position Position;
            public readonly Rotation Rotation;
            public readonly SquadMemberCount MemberCount;

            public SquadData(Position position, Rotation rotation, SquadMemberCount memberCount)
            {
                Position = position;
                Rotation = rotation;
                MemberCount = memberCount;
            }
        }
    }
}
