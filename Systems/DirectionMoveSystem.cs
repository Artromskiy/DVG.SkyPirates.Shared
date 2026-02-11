using Arch.Core;
using DVG.SkyPirates.Shared.Components.Config;
using DVG.SkyPirates.Shared.Components.Runtime;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;

namespace DVG.SkyPirates.Shared.Systems
{
    public class DirectionMoveSystem : ITickableExecutor
    {
        private readonly QueryDescription _desc = new QueryDescription().WithAll<Position, Direction, MaxSpeed>().NotDisposing();
        private readonly World _world;
        public DirectionMoveSystem(World world)
        {
            _world = world;
        }

        public void Tick(int tick, fix deltaTime)
        {
            var query = new MoveQuery(deltaTime);
            _world.InlineQuery<MoveQuery, Position, Direction, MaxSpeed>(_desc, ref query);
        }

        private readonly struct MoveQuery : IForEach<Position, Direction, MaxSpeed>
        {
            private readonly fix _deltaTime;

            public MoveQuery(fix deltaTime)
            {
                _deltaTime = deltaTime;
            }

            public readonly void Update(ref Position position, ref Direction direction, ref MaxSpeed maxSpeed)
            {
                var deltaMove = (direction.Value * maxSpeed.Value * _deltaTime).x_y;
                position.Value += deltaMove;
            }
        }
    }
}
