using Arch.Core;
using DVG.SkyPirates.Shared.Components;
using DVG.SkyPirates.Shared.Components.Data;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;

namespace DVG.SkyPirates.Shared.Services.TickableExecutors.Systems
{
    public class DamageSystem : ITickableExecutor
    {
        private readonly QueryDescription _desc = new QueryDescription().WithAll<Health, RecivedDamage>();
        private readonly World _world;

        public DamageSystem(World world)
        {
            _world = world;
        }

        public void Tick(int tick, fix deltaTime)
        {
            var query = new ReciveDamageQuery();
            _world.InlineQuery<ReciveDamageQuery, Health, RecivedDamage>(_desc, ref query);
        }

        private readonly struct ReciveDamageQuery :
            IForEach<Health, RecivedDamage>
        {

            public void Update(ref Health health, ref RecivedDamage recivedDamage)
            {
                health.Value -= recivedDamage.Value;
                recivedDamage.Value = 0;
            }
        }
    }
}
