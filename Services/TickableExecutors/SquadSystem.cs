using Arch.Core;
using Arch.Core.Extensions;
using DVG.SkyPirates.Shared.Archetypes;
using DVG.SkyPirates.Shared.Components;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;

namespace DVG.SkyPirates.Shared.Services.TickableExecutors
{
    public class SquadSystem : ITickableExecutor
    {
        private const int SquadSpeed = 7;
        private readonly World _world;
        public SquadSystem(World world)
        {
            _world = world;
        }

        public void Tick(int tick, fix deltaTime)
        {
            var query = new SquadQuery(deltaTime);
            _world.InlineQuery<SquadQuery, Squad, Position, Direction, Fixation>
                (SquadArch.GetQuery(), ref query);
        }

        private readonly struct SquadQuery : IForEach<Squad, Position, Direction, Fixation>
        {
            private readonly fix _deltaTime;

            public SquadQuery(fix deltaTime)
            {
                _deltaTime = deltaTime;
            }

            public readonly void Update(ref Squad squad, ref Position position, ref Direction direction, ref Fixation fixation)
            {
                var deltaMove = (direction.Value * SquadSpeed * _deltaTime).x_y;
                position.Value += deltaMove;

                for (int i = 0; i < squad.units.Count; i++)
                {
                    var positions = squad.positions;
                    var order = squad.orders[i];
                    var offset = positions[order].x_y;
                    squad.units[i].Get<TargetPosition>().Value = position.Value + offset;
                    squad.units[i].Get<Fixation>().Value = fixation.Value;
                }
            }
        }
    }
}
