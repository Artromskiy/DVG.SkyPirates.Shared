using Arch.Core;
using DVG.Core;
using DVG.SkyPirates.Shared.Components;
using DVG.SkyPirates.Shared.Components.Data;
using DVG.SkyPirates.Shared.Data;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Systems
{
    public sealed class SquadUnitsSystem : ITickableExecutor
    {
        private const int SquadSearchTarget = 7;
        private readonly QueryDescription _desc = new QueryDescription().WithAll<Squad, Position, Fixation, Rotation>();

        private readonly Dictionary<int, PackedCirclesConfig> _circlesConfigCache = new();

        private readonly IPathFactory<PackedCirclesConfig> _circlesModelFactory;
        private readonly World _world;

        public SquadUnitsSystem(IPathFactory<PackedCirclesConfig> circlesModelFactory, World world)
        {
            _circlesModelFactory = circlesModelFactory;
            _world = world;
        }

        public void Tick(int tick, fix deltaTime)
        {
            var query = new SquadUnitsQuery(_circlesConfigCache, _circlesModelFactory, _world);
            _world.InlineQuery<SquadUnitsQuery, Squad, Position, Fixation, Rotation>(_desc, ref query);
        }

        private readonly struct SquadUnitsQuery : IForEach<Squad, Position, Fixation, Rotation>
        {
            private readonly Dictionary<int, PackedCirclesConfig> _circlesConfigCache;
            private readonly IPathFactory<PackedCirclesConfig> _circlesModelFactory;
            private readonly World _world;

            public SquadUnitsQuery(Dictionary<int, PackedCirclesConfig> circlesConfigCache, IPathFactory<PackedCirclesConfig> circlesModelFactory, World world)
            {
                _circlesConfigCache = circlesConfigCache;
                _circlesModelFactory = circlesModelFactory;
                _world = world;
            }

            public readonly void Update(ref Squad squad, ref Position position, ref Fixation fixation, ref Rotation rotation, ref TargetSearchData targetSearchData)
            {
                if (squad.units.Count == 0)
                    return;

                var packedCircles = GetCirclesConfig(squad.units.Count);

                for (int i = 0; i < squad.units.Count; i++)
                {
                    var localPoint = packedCircles.Points[i] / 2;
                    var unit = squad.units[i];

                    var entityData = _world.GetEntityData(unit);
                    entityData.Get<TargetSearchData>().Position = targetSearchData.Position;
                    entityData.Get<TargetSearchData>().Distance = fixation.Value ? 0 : targetSearchData.Distance;

                    entityData.Get<Destination>().Position = position.Value + localPoint.x_y;
                    entityData.Get<Destination>().Rotation = rotation.Value;
                }
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