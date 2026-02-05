using Arch.Core;
using DVG.SkyPirates.Shared.Components;
using DVG.SkyPirates.Shared.Components.Config; using DVG.SkyPirates.Shared.Components.Framed; using DVG.SkyPirates.Shared.Components.Runtime;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;

namespace DVG.SkyPirates.Shared.Systems
{
    /// <summary>
    /// Moves squad and sets unit's
    /// <see href="Destination"/>, <see href="Fixation"/>, <see href="TargetSearchData"/>
    /// </summary>
    public sealed class SquadMoveSystem : ITickableExecutor
    {
        private readonly QueryDescription _desc = new QueryDescription().WithAll<Squad, Position, Direction>();
        private const int SquadSpeed = 7;
        private readonly World _world;
        public SquadMoveSystem(World world)
        {
            _world = world;
        }

        public void Tick(int tick, fix deltaTime)
        {
            var query = new SquadQuery(deltaTime);
            _world.InlineQuery<SquadQuery, Squad, Position, Direction>(_desc, ref query);
        }

        private readonly struct SquadQuery : IForEach<Squad, Position, Direction>
        {
            private readonly fix _deltaTime;

            public SquadQuery(fix deltaTime)
            {
                _deltaTime = deltaTime;
            }

            public readonly void Update(ref Squad squad, ref Position position, ref Direction direction)
            {
                var deltaMove = (direction.Value * SquadSpeed * _deltaTime).x_y;
                position.Value += deltaMove;
            }
        }
    }
}
