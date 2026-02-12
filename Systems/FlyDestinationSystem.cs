using Arch.Core;
using DVG.SkyPirates.Shared.Components.Runtime;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;

namespace DVG.SkyPirates.Shared.Systems
{
    public class FlyDestinationSystem : ITickableExecutor
    {
        private readonly QueryDescription _desc = new QueryDescription().
            WithAll<Position, FlyDestination>().NotDisposing();

        private readonly World _world;


        public FlyDestinationSystem(World world)
        {
            _world = world;
        }

        public void Tick(int tick, fix deltaTime)
        {
            var query = new FlyQueryQuery(deltaTime);
            _world.InlineQuery<FlyQueryQuery, Position, FlyDestination>(in _desc, ref query);
        }

        private readonly struct FlyQueryQuery : IForEach<Position, FlyDestination>
        {
            private readonly fix DeltaTime;

            public FlyQueryQuery(fix deltaTime)
            {
                DeltaTime = deltaTime;
            }

            public void Update(ref Position position, ref FlyDestination fly)
            {
                fix flySpeed = 5;
                fix ArcHeight = 2;

                var end = fly.EndPosition;
                var start = fly.StartPosition;

                var nextPos = fix3.MoveTowards(position, end, DeltaTime * flySpeed);
                var totalDist = fix3.Distance(start, end);
                var currentDist = fix3.Distance(start, nextPos);
                fix t = Maths.InvLerp(0, totalDist, currentDist);
                var arc = 4 * t * (1 - t);
                nextPos.y += ArcHeight * arc;

                position = nextPos;
            }
        }
    }
}