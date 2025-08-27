using Arch.Core;
using DVG.SkyPirates.Shared.Components;
using DVG.SkyPirates.Shared.Components.Data;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;

namespace DVG.SkyPirates.Shared.Services.TickableExecutors.Systems
{
    public class SquadUnitsSystem : ITickableExecutor
    {
        private readonly QueryDescription _desc = new QueryDescription().WithAll<Squad, Position, Fixation, Rotation>();
        private const int SquadSearchTarget = 10;
        private readonly World _world;
        public SquadUnitsSystem(World world)
        {
            _world = world;
        }

        public void Tick(int tick, fix deltaTime)
        {
            var query = new SquadUnitsQuery(_world);
            _world.InlineQuery<SquadUnitsQuery, Squad, Position, Fixation, Rotation>(_desc, ref query);
        }

        private readonly struct SquadUnitsQuery : IForEach<Squad, Position, Fixation, Rotation>
        {
            private readonly World _world;

            public SquadUnitsQuery(World world)
            {
                _world = world;
            }

            public readonly void Update(ref Squad squad, ref Position position, ref Fixation fixation, ref Rotation rotation)
            {
                for (int i = 0; i < squad.units.Count; i++)
                {
                    var positions = squad.positions;
                    var order = squad.orders[i];
                    var offset = positions[order].x_y;
                    var unit = squad.units[i];

                    var entityData = _world.GetEntityData(unit);
                    entityData.Get<TargetSearchData>().Position = position.Value;
                    entityData.Get<TargetSearchData>().Distance = fixation.Value ? 0 : SquadSearchTarget;
                    entityData.Get<Destination>().Position = position.Value + offset;
                    entityData.Get<Destination>().Rotation = rotation.Value;
                }
            }
        }
    }
}
