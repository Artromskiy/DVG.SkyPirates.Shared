using Arch.Core;
using DVG.SkyPirates.Shared.Components;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;

namespace DVG.SkyPirates.Shared.Systems
{
    public class ClearSystem : ITickableExecutor
    {
        private readonly World _world;

        private readonly QueryDescription _separationForceDesc = new QueryDescription().WithAll<SeparationForce>();
        private readonly QueryDescription _recivedDamageDesc = new QueryDescription().WithAll<RecivedDamage>();

        public ClearSystem(World world)
        {
            _world = world;
        }

        public void Tick(int tick, fix deltaTime)
        {
            _world.Set<SeparationForce>(_separationForceDesc, default);
            _world.Set<RecivedDamage>(_recivedDamageDesc, default);
        }
    }
}
