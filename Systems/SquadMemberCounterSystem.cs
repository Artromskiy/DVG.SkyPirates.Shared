using Arch.Core;
using DVG.Core.Collections;
using DVG.SkyPirates.Shared.Components.Framed;
using DVG.SkyPirates.Shared.Components.Runtime;
using DVG.SkyPirates.Shared.Components.Special;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;
using DVG.SkyPirates.Shared.Systems.Special;

namespace DVG.SkyPirates.Shared.Systems
{
    public class SquadMemberCounterSystem : ITickableExecutor
    {
        private readonly QueryDescription _unitsDesc = new QueryDescription().
            WithAll<SquadMember>().NotDisposing();

        private readonly QueryDescription _squadsDesc = new QueryDescription().
            WithAll<Squad, SyncId>().NotDisposing();

        private readonly World _world;

        private readonly Lookup<int> _unitCountPerSquad = new();

        public SquadMemberCounterSystem(World world)
        {
            _world = world;
        }

        public void Tick(int tick, fix deltaTime)
        {
            _unitCountPerSquad.Clear();

            var countQuery = new CountUnitsQuery(_unitCountPerSquad);
            _world.InlineQuery<CountUnitsQuery, SquadMember>(_unitsDesc, ref countQuery);

            var squadQuery = new ApplyCountQuery(_unitCountPerSquad);
            _world.InlineQuery<ApplyCountQuery, SyncId, SquadMemberCount>(
                _squadsDesc, ref squadQuery);
        }

        private readonly struct CountUnitsQuery : IForEach<SquadMember>
        {
            private readonly Lookup<int> _map;

            public CountUnitsQuery(Lookup<int> map)
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

        private readonly struct ApplyCountQuery : IForEach<SyncId, SquadMemberCount>
        {
            private readonly Lookup<int> _unitCounts;

            public ApplyCountQuery(Lookup<int> unitCounts)
            {
                _unitCounts = unitCounts;
            }

            public void Update(
                ref SyncId syncId,
                ref SquadMemberCount memberCount)
            {
                _unitCounts.TryGetValue(syncId.Value, out var count);
                memberCount.Value = count;
            }
        }
    }
}
