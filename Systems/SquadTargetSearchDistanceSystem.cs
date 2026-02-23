using Arch.Core;
using DVG.Components;
using DVG.SkyPirates.Shared.Components.Config;
using DVG.SkyPirates.Shared.Components.Framed;
using DVG.SkyPirates.Shared.Components.Runtime;
using DVG.SkyPirates.Shared.IFactories;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Systems
{
    public class SquadTargetSearchDistanceSystem : ITickableExecutor
    {
        private readonly QueryDescription _squadsMembers = new QueryDescription().
            WithAll<SquadMember, ImpactDistance>();
        private readonly QueryDescription _squadsDesc = new QueryDescription().
            WithAll<Squad, SquadMemberCount, Fixation, TargetSearchDistance>().NotDisposing().NotDisabled();

        private readonly Dictionary<int, fix> _rangePerSquad = new();
        private readonly IPackedCirclesFactory _packedCirclesFactory;
        private readonly World _world;


        public SquadTargetSearchDistanceSystem(IPackedCirclesFactory packedCirclesFactory, World world)
        {
            _packedCirclesFactory = packedCirclesFactory;
            _world = world;
        }

        public void Tick(int tick, fix deltaTime)
        {
            _rangePerSquad.Clear();
            _world.Query(in _squadsMembers, (ref SquadMember member, ref ImpactDistance impactDistance) =>
            {
                _rangePerSquad[member.SquadId] =
                    Maths.Max(_rangePerSquad.GetValueOrDefault(member.SquadId), impactDistance);
            });

            var squadQuery = new ApplyRangeQuery(_packedCirclesFactory, _rangePerSquad);
            _world.InlineQuery<ApplyRangeQuery, SyncId, Fixation, SquadMemberCount, TargetSearchDistance>(
                _squadsDesc, ref squadQuery);
        }

        private readonly struct ApplyRangeQuery
            : IForEach<SyncId, Fixation, SquadMemberCount, TargetSearchDistance>
        {
            private readonly IPackedCirclesFactory _factory;
            private readonly Dictionary<int, fix> _rangePerSquad;

            public ApplyRangeQuery(IPackedCirclesFactory factory, Dictionary<int, fix> rangePerSquad)
            {
                _factory = factory;
                _rangePerSquad = rangePerSquad;
            }

            public void Update(
                ref SyncId syncId,
                ref Fixation fixation,
                ref SquadMemberCount memberCount,
                ref TargetSearchDistance searchDistance)
            {
                fix addRadius = memberCount == 0 ? 0 : _factory.Create(memberCount).Radius;
                searchDistance = fixation ? 0 : _rangePerSquad.GetValueOrDefault(syncId) + addRadius;
            }
        }
    }
}