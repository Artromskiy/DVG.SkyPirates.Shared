using Arch.Core;
using DVG.SkyPirates.Shared.Components.Config;
using DVG.SkyPirates.Shared.Components.Runtime;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;

namespace DVG.SkyPirates.Shared.Systems
{
    public class EnsureSystem : ITickableExecutor
    {
        private readonly World _world;
        public EnsureSystem(World world)
        {
            _world = world;
        }

        public void Tick(int tick, fix deltaTime)
        {
            _world.AddQuery((ref MaxHealth maxHealth, ref Health health) =>
                health = (fix)maxHealth);
        }
    }
}
