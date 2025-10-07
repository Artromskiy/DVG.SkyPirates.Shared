using Arch.Core;
using DVG.Core;
using DVG.SkyPirates.Shared.Components;
using DVG.SkyPirates.Shared.Components.Data;
using DVG.SkyPirates.Shared.Data;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Systems
{
    public class SquadRangeSystem : ITickableExecutor
    {
        private const int SquadSearchTarget = 5;
        private readonly QueryDescription _desc = new QueryDescription().WithAll<Squad, Position, TargetSearchData>();

        private readonly Dictionary<int, PackedCirclesConfig> _circlesConfigCache = new();

        private readonly IPathFactory<PackedCirclesConfig> _circlesModelFactory;
        private readonly World _world;

        public SquadRangeSystem(IPathFactory<PackedCirclesConfig> circlesModelFactory, World world)
        {
            _circlesModelFactory = circlesModelFactory;
            _world = world;
        }

        public void Tick(int tick, fix deltaTime)
        {
            var query = new SquadUnitsQuery(_circlesConfigCache, _circlesModelFactory);
            _world.InlineQuery<SquadUnitsQuery, Squad, Position, TargetSearchData>(_desc, ref query);
        }

        private readonly struct SquadUnitsQuery : IForEach<Squad, Position, TargetSearchData>
        {
            private readonly Dictionary<int, PackedCirclesConfig> _circlesConfigCache;
            private readonly IPathFactory<PackedCirclesConfig> _circlesModelFactory;

            public SquadUnitsQuery(Dictionary<int, PackedCirclesConfig> circlesConfigCache, IPathFactory<PackedCirclesConfig> circlesModelFactory)
            {
                _circlesConfigCache = circlesConfigCache;
                _circlesModelFactory = circlesModelFactory;
            }

            public readonly void Update(ref Squad squad, ref Position position, ref TargetSearchData targetSearchData)
            {
                if (squad.units.Count == 0)
                    return;

                var packedCircles = GetCirclesConfig(squad.units.Count);
                targetSearchData.Position = position.Value;
                targetSearchData.Distance = SquadSearchTarget + packedCircles.Radius / 2;
            }

            private PackedCirclesConfig GetCirclesConfig(int count)
            {
                if (!_circlesConfigCache.TryGetValue(count, out var config))
                    _circlesConfigCache[count] = config = _circlesModelFactory.Create("Configs/PackedCircles/PackedCirclesModel" + count);
                return config;
            }
        }
    }
}