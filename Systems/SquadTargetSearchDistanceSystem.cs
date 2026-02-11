using Arch.Core;
using DVG.SkyPirates.Shared.Components.Config;
using DVG.SkyPirates.Shared.Components.Framed;
using DVG.SkyPirates.Shared.Components.Runtime;
using DVG.SkyPirates.Shared.IFactories;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;

namespace DVG.SkyPirates.Shared.Systems
{
    public class SquadTargetSearchDistanceSystem : ITickableExecutor
    {
        private const int BaseRange = 3;

        private readonly QueryDescription _squadsDesc = new QueryDescription().
            WithAll<Squad, SquadMemberCount, Fixation, TargetSearchDistance>().NotDisposing();

        private readonly IPackedCirclesFactory _packedCirclesFactory;
        private readonly World _world;


        public SquadTargetSearchDistanceSystem(IPackedCirclesFactory packedCirclesFactory, World world)
        {
            _packedCirclesFactory = packedCirclesFactory;
            _world = world;
        }

        public void Tick(int tick, fix deltaTime)
        {
            var squadQuery = new ApplyRangeQuery(_packedCirclesFactory);
            _world.InlineQuery<ApplyRangeQuery, Fixation, SquadMemberCount, TargetSearchDistance>(
                _squadsDesc, ref squadQuery);
        }

        private readonly struct ApplyRangeQuery
            : IForEach<Fixation, SquadMemberCount, TargetSearchDistance>
        {
            private readonly IPackedCirclesFactory _factory;

            public ApplyRangeQuery(IPackedCirclesFactory factory)
            {
                _factory = factory;
            }

            public void Update(
                ref Fixation fixation,
                ref SquadMemberCount memberCount,
                ref TargetSearchDistance searchDistance)
            {
                fix addRadius = memberCount.Value == 0 ? 0 :
                    _factory.Create(memberCount.Value).Radius;
                searchDistance.Value = fixation.Value ? 0 : BaseRange + addRadius;
            }
        }
    }
}