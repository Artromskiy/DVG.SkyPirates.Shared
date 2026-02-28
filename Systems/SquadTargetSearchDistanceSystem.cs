using Arch.Core;
using DVG.Components;
using DVG.SkyPirates.Shared.Components.Config;
using DVG.SkyPirates.Shared.Components.Framed;
using DVG.SkyPirates.Shared.Components.Runtime;
using DVG.SkyPirates.Shared.Data;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Systems
{
    public class SquadTargetSearchDistanceSystem : ITickableExecutor
    {
        private static readonly fix _baseRange = 1;

        private readonly QueryDescription _squadsMembers = new QueryDescription().
            WithAll<SquadMember, ImpactDistance>().NotDisposing().NotDisabled();

        private readonly QueryDescription _squadsDesc = new QueryDescription().
            WithAll<Squad, SquadMemberCount, TargetSearchDistance>().NotDisposing().NotDisabled();

        private readonly Dictionary<int, fix> _maxImpactDistancePerSquad = new();
        private readonly PackedCirclesConfig _circlesConfig;
        private readonly World _world;


        public SquadTargetSearchDistanceSystem(PackedCirclesConfig circlesConfig, World world)
        {
            _circlesConfig = circlesConfig;
            _world = world;
        }

        public void Tick(int tick, fix deltaTime)
        {
            _maxImpactDistancePerSquad.Clear();
            var impactQuery = new CollectImpactDistancesQuery(_maxImpactDistancePerSquad);
            _world.InlineQuery<CollectImpactDistancesQuery, SquadMember, ImpactDistance>(in _squadsMembers, ref impactQuery);

            var squadQuery = new ApplyRangeQuery(_circlesConfig, _maxImpactDistancePerSquad);
            _world.InlineQuery<ApplyRangeQuery, SyncId, SquadMemberCount, TargetSearchDistance>(
                _squadsDesc, ref squadQuery);
        }

        private readonly struct CollectImpactDistancesQuery : IForEach<SquadMember, ImpactDistance>
        {
            private readonly Dictionary<int, fix> _maxImpactDistancePerSquad;

            public CollectImpactDistancesQuery(Dictionary<int, fix> maxImpactDistancePerSquad)
            {
                _maxImpactDistancePerSquad = maxImpactDistancePerSquad;
            }

            public void Update(ref SquadMember member, ref ImpactDistance impactDistance)
            {
                var currentImpactDistance = _maxImpactDistancePerSquad.GetValueOrDefault(member.SquadId);
                _maxImpactDistancePerSquad[member.SquadId] =
                    Maths.Max(currentImpactDistance, impactDistance);
            }
        }

        private readonly struct ApplyRangeQuery
            : IForEach<SyncId, SquadMemberCount, TargetSearchDistance>
        {
            private readonly PackedCirclesConfig _circlesConfig;
            private readonly Dictionary<int, fix> _maxImpactDistancePerSquad;

            public ApplyRangeQuery(PackedCirclesConfig circlesConfig, Dictionary<int, fix> maxImpactDistancePerSquad)
            {
                _circlesConfig = circlesConfig;
                _maxImpactDistancePerSquad = maxImpactDistancePerSquad;
            }

            public void Update(
                ref SyncId syncId,
                ref SquadMemberCount memberCount,
                ref TargetSearchDistance searchDistance)
            {
                fix squadRadius = memberCount == 0 ? 0 : _circlesConfig[memberCount].Radius;
                var maxImpactDistance = _maxImpactDistancePerSquad.GetValueOrDefault(syncId);
                searchDistance = maxImpactDistance + _baseRange + squadRadius;
            }
        }
    }
}