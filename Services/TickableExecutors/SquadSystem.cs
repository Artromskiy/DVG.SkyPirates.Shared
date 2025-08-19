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

            public readonly void Update(ref Squad s, ref Position p, ref Direction d, ref Fixation f)
            {
                var deltaMove = (d.direction * SquadSpeed * _deltaTime).x_y;
                p.position += deltaMove;

                for (int i = 0; i < s.units.Count; i++)
                {
                    var offset = s.positions[s.orders[i]].x_y;
                    s.units[i].Get<Unit>().TargetPosition = p.position + offset;
                    s.units[i].Get<Fixation>().fixation = f.fixation;
                }
            }
        }
    }
}
