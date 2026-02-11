using Arch.Core;
using DVG.SkyPirates.Shared.Components.Framed;
using DVG.SkyPirates.Shared.Components.Runtime;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;

namespace DVG.SkyPirates.Shared.Systems
{
    public class ClearSystem : ITickableExecutor
    {
        private readonly World _world;

        private readonly QueryDescription _destinationQuery = new QueryDescription().WithAll<Destination, Position, Rotation>().NotDisposing();
        private readonly QueryDescription _recivedDamageDesc = new QueryDescription().WithAll<RecivedDamage>().NotDisposing();
        private readonly QueryDescription _targetDesc = new QueryDescription().WithAll<Target>().NotDisposing();
        private readonly QueryDescription _targetsDesc = new QueryDescription().WithAll<Targets>().NotDisposing();

        public ClearSystem(World world)
        {
            _world = world;
        }

        public void Tick(int tick, fix deltaTime)
        {
            _world.Set<RecivedDamage>(in _recivedDamageDesc, default);
            _world.Set<Target>(in _targetDesc, default);
            _world.Set<Targets>(in _targetsDesc, default);

            _world.Query(in _destinationQuery, (ref Destination destination, ref Position position, ref Rotation rotation) =>
            {
                destination.Position = position.Value;
                destination.Rotation = rotation.Value;
            });
        }
    }
}
