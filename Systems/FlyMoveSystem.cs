using Arch.Core;
using DVG.SkyPirates.Shared.Components.Config;
using DVG.SkyPirates.Shared.Components.Runtime;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;

namespace DVG.SkyPirates.Shared.Systems
{
    public class FlyMoveSystem : ITickableExecutor
    {
        private readonly QueryDescription _desc = new QueryDescription().
            WithAll<Position, FlyDestination, MaxSpeed>().NotDisposing();

        private readonly World _world;


        public FlyMoveSystem(World world)
        {
            _world = world;
        }

        public void Tick(int tick, fix deltaTime)
        {
            var query = new FlyQueryQuery(deltaTime);
            _world.InlineQuery<FlyQueryQuery, Position, FlyDestination, MaxSpeed>(in _desc, ref query);
        }

        private readonly struct FlyQueryQuery : IForEach<Position, FlyDestination, MaxSpeed>
        {
            private readonly fix DeltaTime;

            public FlyQueryQuery(fix deltaTime)
            {
                DeltaTime = deltaTime;
            }

            public void Update(ref Position position, ref FlyDestination fly, ref MaxSpeed maxSpeed)
            {
                fix ArcHeight = 3;

                var end = fly.EndPosition;
                var start = fly.StartPosition;
                var endXZ = end.xz;
                var startXZ = start.xz;
                var currentXZ = fix2.MoveTowards(((fix3)position).xz, endXZ, DeltaTime * maxSpeed);
                var totalDistXZ = fix2.Distance(startXZ, endXZ);
                var currentDistXZ = fix2.Distance(startXZ, currentXZ);
                var percent = Maths.InvLerp(0, totalDistXZ, currentDistXZ);
                var currentY = Maths.Lerp(start.y, end.y, percent);

                var arc = 4 * percent * (1 - percent);
                position = new fix3(currentXZ.x, currentY + arc * ArcHeight, currentXZ.y);
            }
        }
    }
}