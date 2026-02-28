using Arch.Core;
using DVG.SkyPirates.Shared.Components.Config;
using DVG.SkyPirates.Shared.Components.Runtime;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Systems
{
    public class FlyMoveSystem : IDeltaTickableExecutor
    {
        private readonly QueryDescription _desc = new QueryDescription().
            WithAll<Position, FlyDestination, MaxSpeed>().NotDisposing().NotDisabled();

        private readonly List<Entity> _finished = new();

        private readonly World _world;

        public FlyMoveSystem(World world)
        {
            _world = world;
        }

        public void Tick(int tick, fix deltaTime)
        {
            _finished.Clear();
            var flyQuery = new FlyQuery(deltaTime);
            _world.InlineEntityQuery<FlyQuery, Position, FlyDestination, MaxSpeed>(in _desc, ref flyQuery);
            var flyEndQuery = new FlyEndQuery(_finished);
            _world.InlineEntityQuery<FlyEndQuery, Position, FlyDestination, MaxSpeed>(in _desc, ref flyEndQuery);
            foreach (var item in _finished)
                _world.Remove<FlyDestination>(item);
        }

        private readonly struct FlyEndQuery : IForEachWithEntity<Position, FlyDestination, MaxSpeed>
        {
            private readonly List<Entity> _finished;

            public FlyEndQuery(List<Entity> finished)
            {
                _finished = finished;
            }

            public void Update(Entity entity, ref Position position, ref FlyDestination fly, ref MaxSpeed maxSpeed)
            {
                if (position == fly.EndPosition)
                    _finished.Add(entity);
            }
        }

        private readonly struct FlyQuery : IForEachWithEntity<Position, FlyDestination, MaxSpeed>
        {
            private readonly fix DeltaTime;

            public FlyQuery(fix deltaTime)
            {
                DeltaTime = deltaTime;
            }

            public void Update(Entity entity, ref Position position, ref FlyDestination fly, ref MaxSpeed maxSpeed)
            {
                const int ArcHeight = 4;

                var end = fly.EndPosition;
                var start = fly.StartPosition;
                var endXZ = end.xz;
                var startXZ = start.xz;
                var currentXZ = fix2.MoveTowards(((fix3)position).xz, endXZ, DeltaTime * maxSpeed);
                var totalDistXZ = fix2.Distance(startXZ, endXZ);
                var currentDistXZ = fix2.Distance(startXZ, currentXZ);
                var percent = totalDistXZ == 0 ? 1 : Maths.InvLerp(0, totalDistXZ, currentDistXZ);
                var currentY = Maths.Lerp(start.y, end.y, percent);

                var arc = 4 * percent * (1 - percent);
                position = new fix3(currentXZ.x, currentY + arc * ArcHeight, currentXZ.y);

            }
        }
    }
}