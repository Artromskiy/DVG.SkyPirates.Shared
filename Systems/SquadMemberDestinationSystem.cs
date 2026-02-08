using Arch.Core;
using DVG.Core.Collections;
using DVG.SkyPirates.Shared.Components.Framed;
using DVG.SkyPirates.Shared.Components.Runtime;
using DVG.SkyPirates.Shared.Components.Special;
using DVG.SkyPirates.Shared.IFactories;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;
using DVG.SkyPirates.Shared.Systems.Special;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Systems
{
    public class SquadMemberDestinationSystem : ITickableExecutor
    {
        private readonly QueryDescription _unitsDesc = new QueryDescription().
            WithAll<SquadMember, Destination>().NotDisposing();

        private readonly QueryDescription _squadsDesc = new QueryDescription().
            WithAll<Squad>().NotDisposing();

        private readonly World _world;
        private readonly IPackedCirclesFactory _packedCirclesFactory;

        private readonly Lookup<int> _orderPerUnit = new();
        private readonly Lookup<SquadData> _dataPerSquad = new();
        private readonly Dictionary<int, List<SyncId>> _unitsPerSquad = new();

        public SquadMemberDestinationSystem(World world, IPackedCirclesFactory packedCirclesFactory)
        {
            _world = world;
            _packedCirclesFactory = packedCirclesFactory;
        }

        public void Tick(int tick, fix deltaTime)
        {
            _orderPerUnit.Clear();
            _dataPerSquad.Clear();
            _unitsPerSquad.Clear();

            var collectSquadsQuery = new CollectDataPerSquadQuery(_dataPerSquad);
            _world.InlineQuery<CollectDataPerSquadQuery, SyncId, Position, Rotation, SquadMemberCount>(_squadsDesc, ref collectSquadsQuery);

            var collectUnitsQuery = new CollectUnitsPerSquadQuery(_unitsPerSquad);
            _world.InlineQuery<CollectUnitsPerSquadQuery, SquadMember, SyncId>(_unitsDesc, ref collectUnitsQuery);

            foreach (var item in _unitsPerSquad)
            {
                item.Value.Sort((u1, u2) => u1.Value.CompareTo(u2.Value));
                for (var i = 0; i < item.Value.Count; i++)
                {
                    _orderPerUnit[item.Value[i].Value] = i;
                }
            }

            var applyQuery = new ApplySquadMembersDestinationQuery(_orderPerUnit, _dataPerSquad, _packedCirclesFactory);
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

            public CollectUnitsPerSquadQuery(Dictionary<int, List<SyncId>> map)
            {
                _map = map;
            }

            public void Update(ref SquadMember member, ref SyncId syncId)
            {
                if (!_map.TryGetValue(member.SquadId, out var list))
                    _map[member.SquadId] = list = new(8);

                list.Add(syncId);
            }
        }

        private readonly struct ApplySquadMembersDestinationQuery
            : IForEach<SyncId, SquadMember, Destination>
        {
            private readonly Lookup<int> _orderPerUnit;
            private readonly Lookup<SquadData> _dataPerSquad;
            private readonly IPackedCirclesFactory _factory;

            public ApplySquadMembersDestinationQuery(Lookup<int> orderPerUnit, Lookup<SquadData> dataPerSquad, IPackedCirclesFactory factory)
            {
                _orderPerUnit = orderPerUnit;
                _dataPerSquad = dataPerSquad;
                _factory = factory;
            }

            // Get squad position, offset from it, set as destination
            public void Update(
                ref SyncId syncId,
                ref SquadMember member,
                ref Destination destination)
            {
                var unitsCount = _dataPerSquad[member.SquadId].MemberCount.Value;
                var circles = _factory.Create(unitsCount);
                var order = _orderPerUnit[syncId.Value];
                var local = circles.Points[order] / 2;
                destination.Position = _dataPerSquad[member.SquadId].Position.Value + local.x_y;
                destination.Rotation = _dataPerSquad[member.SquadId].Rotation.Value;
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
