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
            WithAll<SquadMember, ImpactDistance>();
        private readonly QueryDescription _squadsDesc = new QueryDescription().
            WithAll<Squad, SquadMemberCount, Fixation, TargetSearchDistance>().NotDisposing().NotDisabled();

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
            _world.Query(in _squadsMembers, (ref SquadMember member, ref ImpactDistance impactDistance) =>
            {
                _maxImpactDistancePerSquad[member.SquadId] =
                    Maths.Max(_maxImpactDistancePerSquad.GetValueOrDefault(member.SquadId), impactDistance);
            });

            var squadQuery = new ApplyRangeQuery(_circlesConfig, _maxImpactDistancePerSquad);
            _world.InlineQuery<ApplyRangeQuery, SyncId, Fixation, SquadMemberCount, TargetSearchDistance>(
                _squadsDesc, ref squadQuery);
        }

        private readonly struct ApplyRangeQuery
            : IForEach<SyncId, Fixation, SquadMemberCount, TargetSearchDistance>
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
                ref Fixation fixation,
                ref SquadMemberCount memberCount,
                ref TargetSearchDistance searchDistance)
            {
                fix squadRadius = memberCount == 0 ? 0 : _circlesConfig[memberCount].Radius;
                var maxImpactDistance = _maxImpactDistancePerSquad.GetValueOrDefault(syncId);
                searchDistance = fixation ? 0 : maxImpactDistance + _baseRange + squadRadius;
            }
        }
    }
}