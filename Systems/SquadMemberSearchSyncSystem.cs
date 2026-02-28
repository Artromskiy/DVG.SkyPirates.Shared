using Arch.Core;
using DVG.Components;
using DVG.Core.Collections;
using DVG.SkyPirates.Shared.Components.Config;
using DVG.SkyPirates.Shared.Components.Framed;
using DVG.SkyPirates.Shared.Components.Runtime;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;

namespace DVG.SkyPirates.Shared.Systems
{
    public class SquadMemberSearchSyncSystem : ITickableExecutor
    {
        private readonly QueryDescription _unitsDesc = new QueryDescription().
            WithAll<SquadMember, TargetSearchDistance, TargetSearchPosition>().
            NotDisposing().NotDisabled();

        private readonly QueryDescription _squadsDesc = new QueryDescription().
            WithAll<Squad, SyncId, TargetSearchDistance, TargetSearchPosition, Fixation>().
            NotDisposing().NotDisabled();

        private readonly World _world;

        private readonly Lookup<TargetSearchData> _searchDataPerSquad = new();

        public SquadMemberSearchSyncSystem(World world)
        {
            _world = world;
        }

        public void Tick(int tick, fix deltaTime)
        {
            _searchDataPerSquad.Clear();

            var cacheQuery = new CacheSquadSearchQuery(_searchDataPerSquad);
            _world.InlineQuery<CacheSquadSearchQuery, SyncId, TargetSearchPosition, TargetSearchDistance, Fixation>
                (in _squadsDesc, ref cacheQuery);

            var setDataQuery = new SetMembersTargetSearchDataQuery(_searchDataPerSquad);
            _world.InlineQuery<SetMembersTargetSearchDataQuery, SquadMember, TargetSearchPosition, TargetSearchDistance>
                (_unitsDesc, ref setDataQuery);
        }

        private readonly struct CacheSquadSearchQuery : IForEach<SyncId, TargetSearchPosition, TargetSearchDistance, Fixation>
        {
            private readonly Lookup<TargetSearchData> _searchDataPerSquad;
            public CacheSquadSearchQuery(Lookup<TargetSearchData> searchDataPerSquad)
            {
                _searchDataPerSquad = searchDataPerSquad;
            }

            public void Update(ref SyncId syncId, ref TargetSearchPosition searchPosition, ref TargetSearchDistance searchDistance, ref Fixation fixation)
            {
                TargetSearchDistance distance = fixation ? fix.Zero : searchDistance;
                _searchDataPerSquad[syncId.Value] = new(searchPosition, distance);
            }
        }

        private readonly struct SetMembersTargetSearchDataQuery : IForEach<SquadMember, TargetSearchPosition, TargetSearchDistance>
        {
            private readonly Lookup<TargetSearchData> _searchDataPerSquad;

            public SetMembersTargetSearchDataQuery(Lookup<TargetSearchData> searchDataPerSquad)
            {
                _searchDataPerSquad = searchDataPerSquad;
            }

            public void Update(ref SquadMember squadMember, ref TargetSearchPosition searchPosition, ref TargetSearchDistance searchDistance)
            {
                _searchDataPerSquad.TryGetValue(squadMember.SquadId, out var searchData);
                searchPosition = searchData.TargetSearchPosition;
                searchDistance = searchData.TargetSearchDistance;
            }
        }

        private readonly struct TargetSearchData
        {
            public readonly TargetSearchPosition TargetSearchPosition;
            public readonly TargetSearchDistance TargetSearchDistance;

            public TargetSearchData(TargetSearchPosition targetSearchPosition, TargetSearchDistance targetSearchDistance)
            {
                TargetSearchPosition = targetSearchPosition;
                TargetSearchDistance = targetSearchDistance;
            }
        }
    }
}
