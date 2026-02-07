using Arch.Core;
using DVG.SkyPirates.Shared.Components.Framed;
using DVG.SkyPirates.Shared.Components.Runtime;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;

namespace DVG.SkyPirates.Shared.Systems
{
    public class ClearSystem : ITickableExecutor
    {
        private readonly World _world;

        private readonly QueryDescription _destinationQuery = new QueryDescription().WithAll<Destination, Position, Rotation>();
        private readonly QueryDescription _separationForceDesc = new QueryDescription().WithAll<SeparationForce>();
        private readonly QueryDescription _recivedDamageDesc = new QueryDescription().WithAll<RecivedDamage>();

        public ClearSystem(World world)
        {
            _world = world;
        }

        public void Tick(int tick, fix deltaTime)
        {
            _world.Set<SeparationForce>(in _separationForceDesc, default);
            _world.Set<RecivedDamage>(in _recivedDamageDesc, default);
            _world.Query(in _destinationQuery, (ref Destination destination, ref Position position, ref Rotation rotation) =>
            {
                destination.Position = position.Value;
                destination.Rotation = rotation.Value;
            });
        }
    }
}
