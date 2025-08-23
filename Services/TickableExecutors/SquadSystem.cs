using Arch.Core;
using Arch.Core.Extensions;
using DVG.SkyPirates.Shared.Archetypes;
using DVG.SkyPirates.Shared.Components;
using DVG.SkyPirates.Shared.Components.Data;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;

namespace DVG.SkyPirates.Shared.Services.TickableExecutors
{
    /// <summary>
    /// Moves squad and sets unit's
    /// <see href="Destination"/>, <see href="Fixation"/>, <see href="TargetSearchData"/>
    /// </summary>
    public class SquadSystem : ITickableExecutor
    {
        private const int SquadSpeed = 7;
        private const int SquadSearchTarget = 10;
        private readonly World _world;
        public SquadSystem(World world)
        {
            _world = world;
        }

        public void Tick(int tick, fix deltaTime)
        {
            var query = new SquadQuery(deltaTime);
            _world.InlineQuery<SquadQuery, Squad, Position, Direction, Fixation, Rotation>
                (SquadArch.GetQuery(), ref query);
        }

        private readonly struct SquadQuery : IForEach<Squad, Position, Direction, Fixation, Rotation>
        {
            private readonly fix _deltaTime;

            public SquadQuery(fix deltaTime)
            {
                _deltaTime = deltaTime;
            }

            public readonly void Update(ref Squad squad, ref Position position, ref Direction direction, ref Fixation fixation, ref Rotation rotation)
            {
                var deltaMove = (direction.Value * SquadSpeed * _deltaTime).x_y;
                position.Value += deltaMove;

                for (int i = 0; i < squad.units.Count; i++)
                {
                    var positions = squad.positions;
                    var order = squad.orders[i];
                    var offset = positions[order].x_y;
                    var unit = squad.units[i];

                    unit.Get<Fixation>().Value = fixation.Value;
                    unit.Get<TargetSearchData>().Position = position.Value;
                    unit.Get<TargetSearchData>().Distance = fixation.Value ? 0 : SquadSearchTarget;
                    unit.Get<Destination>().Position = position.Value + offset;
                    unit.Get<Destination>().Rotation = rotation.Value;
                }
            }
        }
    }
}
