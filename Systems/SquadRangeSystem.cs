using Arch.Core;
using DVG.SkyPirates.Shared.Components;
using DVG.SkyPirates.Shared.Components.Config;
using DVG.SkyPirates.Shared.Components.Framed;
using DVG.SkyPirates.Shared.Components.Runtime;
using DVG.SkyPirates.Shared.Components.Special;
using DVG.SkyPirates.Shared.IFactories;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Systems
{
    public class SquadRangeSystem : ITickableExecutor
    {
        private const int BaseRange = 5;

        private readonly QueryDescription _unitsDesc = new QueryDescription().
            WithAll<SquadMember>();

        private readonly QueryDescription _squadsDesc = new QueryDescription().
            WithAll<Squad, Position, Fixation, TargetSearchDistance, TargetSearchPosition>();

        private readonly IPackedCirclesFactory _packedCirclesFactory;
        private readonly World _world;

        private readonly Dictionary<int, int> _unitCountPerSquad = new();

        public SquadRangeSystem(IPackedCirclesFactory packedCirclesFactory, World world)
        {
            _packedCirclesFactory = packedCirclesFactory;
            _world = world;
        }

        public void Tick(int tick, fix deltaTime)
        {
            _unitCountPerSquad.Clear();

            var countQuery = new CountUnitsQuery(_unitCountPerSquad);
            _world.InlineQuery<CountUnitsQuery, SquadMember>(_unitsDesc, ref countQuery);

            var squadQuery = new ApplyRangeQuery(_unitCountPerSquad, _packedCirclesFactory);
            _world.InlineQuery<ApplyRangeQuery, SyncId, Position, Fixation, TargetSearchDistance, TargetSearchPosition>(
                _squadsDesc, ref squadQuery);
        }

        private readonly struct CountUnitsQuery : IForEach<SquadMember>
        {
            private readonly Dictionary<int, int> _map;

            public CountUnitsQuery(Dictionary<int, int> map)
            {
                _map = map;
            }

            public void Update(ref SquadMember member)
            {
                if (!_map.ContainsKey(member.SquadId))
                    _map[member.SquadId] = 0;

                _map[member.SquadId]++;
            }
        }

        private readonly struct ApplyRangeQuery
            : IForEach<SyncId, Position, Fixation, TargetSearchDistance, TargetSearchPosition>
        {
            private readonly Dictionary<int, int> _unitCounts;
            private readonly IPackedCirclesFactory _factory;

            public ApplyRangeQuery(Dictionary<int, int> unitCounts, IPackedCirclesFactory factory)
            {
                _unitCounts = unitCounts;
                _factory = factory;
            }

            public void Update(
                ref SyncId syncId,
                ref Position position,
                ref Fixation fixation,
                ref TargetSearchDistance searchDistance,
                ref TargetSearchPosition searchPosition)
            {
                searchPosition.Value = position.Value;
                searchDistance.Value = 0;

                fix addRadius = 0;
                if (_unitCounts.TryGetValue(syncId.Value, out var unitCount) && unitCount != 0)
                {
                    addRadius = _factory.Create(unitCount).Radius / 2;
                }

                searchDistance.Value = fixation.Value ? 0 : BaseRange + addRadius;
            }
        }
    }
}