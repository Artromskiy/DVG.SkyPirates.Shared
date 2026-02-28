using Arch.Core;
using DVG.SkyPirates.Shared.Components.Framed;
using DVG.SkyPirates.Shared.Components.Runtime;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;

namespace DVG.SkyPirates.Shared.Systems
{
    public class SetDestinationSystem : IDeltaTickableExecutor
    {
        private QueryDescription _desc = new QueryDescription().
            WithAll<Position, Rotation, Destination>().NotDisposing().NotDisabled();

        private readonly World _world;

        public SetDestinationSystem(World world)
        {
            _world = world;
        }

        public void Tick(int tick, fix deltaTime)
        {
            var query = new SetDestinationQuery();
            _world.InlineQuery<SetDestinationQuery, Position, Rotation, Destination>(_desc, ref query);
        }

        private readonly struct SetDestinationQuery : IForEach<Position, Rotation, Destination>
        {
            public readonly void Update(ref Position position, ref Rotation rotation, ref Destination destination)
            {
                destination.Position = position;
                destination.Rotation = rotation;
            }
        }
    }
}
