using Arch.Core;
using DVG.Components;
using DVG.Core.Collections;
using DVG.SkyPirates.Shared.Components.Framed;
using DVG.SkyPirates.Shared.Components.Runtime;
using DVG.SkyPirates.Shared.Data;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Systems
{
    public class SquadMemberDestinationSystem : ITickableExecutor
    {
        private readonly QueryDescription _unitsDesc = new QueryDescription().
            WithAll<SquadMember, Destination>().NotDisposing().NotDisabled();

        private readonly QueryDescription _squadsDesc = new QueryDescription().
            WithAll<Squad>().NotDisposing().NotDisabled();

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

            var collectSquadsQuery = new CollectDataPerSquadQuery(_dataPerSquad);
            _world.InlineQuery<CollectDataPerSquadQuery, SyncId, Position, Rotation, SquadMemberCount>(_squadsDesc, ref collectSquadsQuery);

            var collectUnitsQuery = new CollectUnitsPerSquadQuery(_unitsPerSquad, _unitsCache);
            _world.InlineQuery<CollectUnitsPerSquadQuery, SquadMember, SyncId>(_unitsDesc, ref collectUnitsQuery);

            foreach (var item in _unitsPerSquad)
            {
                item.Value.Sort((u1, u2) => u1.Value.CompareTo(u2.Value));
                for (var i = 0; i < item.Value.Count; i++)
                {
                    _orderPerUnit[item.Value[i].Value] = i;
                }
            }

            var applyQuery = new ApplySquadMembersDestinationQuery(_orderPerUnit, _dataPerSquad, _circlesConfig);
            _world.InlineQuery<ApplySquadMembersDestinationQuery, SyncId, SquadMember, Destination>
                (_unitsDesc, ref applyQuery);
        }

        private readonly struct CollectDataPerSquadQuery : IForEach<SyncId, Position, Rotation, SquadMemberCount>
        {
            private readonly Lookup<SquadData> _map;

            public CollectDataPerSquadQuery(Lookup<SquadData> map)
            {
                _map = map;
            }

            public void Update(ref SyncId syncId, ref Position position, ref Rotation rotation, ref SquadMemberCount memberCount)
            {
                _map[syncId.Value] = new(position, rotation, memberCount);
            }
        }

        private readonly struct CollectUnitsPerSquadQuery : IForEach<SquadMember, SyncId>
        {
            private readonly Dictionary<int, List<SyncId>> _map;
            private readonly Queue<List<SyncId>> _unitsCache;

            public CollectUnitsPerSquadQuery(Dictionary<int, List<SyncId>> map, Queue<List<SyncId>> unitsCache)
            {
                _map = map;
                _unitsCache = unitsCache;
            }

            public void Update(ref SquadMember member, ref SyncId syncId)
            {
                if (!_map.TryGetValue(member.SquadId, out var list))
                    _map[member.SquadId] = _unitsCache.TryDequeue(out list) ? list : list = new(8); // really wtf?

                list.Add(syncId);
            }
        }

        private readonly struct ApplySquadMembersDestinationQuery
            : IForEach<SyncId, SquadMember, Destination>
        {
            private readonly Lookup<int> _orderPerUnit;
            private readonly Lookup<SquadData> _dataPerSquad;
            private readonly PackedCirclesConfig _circlesConfig;

            public ApplySquadMembersDestinationQuery(Lookup<int> orderPerUnit, Lookup<SquadData> dataPerSquad, PackedCirclesConfig circlesConfig)
            {
                _orderPerUnit = orderPerUnit;
                _dataPerSquad = dataPerSquad;
                _circlesConfig = circlesConfig;
            }

            // Get squad position, offset from it, set as destination
            public void Update(
                ref SyncId syncId,
                ref SquadMember member,
                ref Destination destination)
            {
                var unitsCount = _dataPerSquad[member.SquadId].MemberCount;
                var circles = _circlesConfig[unitsCount];
                var order = _orderPerUnit[syncId.Value];
                var local = circles.Points[order];
                destination.Position = _dataPerSquad[member.SquadId].Position + local.x_y;
                destination.Rotation = _dataPerSquad[member.SquadId].Rotation;
            }
        }

        private readonly struct SquadData
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
