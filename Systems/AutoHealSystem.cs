using Arch.Core;
using DVG.SkyPirates.Shared.Components.Config;
using DVG.SkyPirates.Shared.Components.Framed;
using DVG.SkyPirates.Shared.Components.Runtime;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;

namespace DVG.SkyPirates.Shared.Systems
{
    public sealed class AutoHealSystem : ITickableExecutor
    {
        private readonly QueryDescription _desc = new QueryDescription().
            WithAll<Health, MaxHealth, AutoHeal, RecivedDamage>().NotDisposing();

        private readonly World _world;

        public AutoHealSystem(World world)
        {
            _world = world;
        }

        public void Tick(int tick, fix deltaTime)
        {
            var query = new HealQuery(deltaTime);
            _world.InlineQuery<HealQuery, Health, MaxHealth, AutoHeal, RecivedDamage>(_desc, ref query);
        }

        private readonly struct HealQuery :
            IForEach<Health, MaxHealth, AutoHeal, RecivedDamage>
        {
            private readonly fix _deltaTime;

            public HealQuery(fix deltaTime)
            {
                _deltaTime = deltaTime;
            }

            public void Update(ref Health health, ref MaxHealth maxHealth, ref AutoHeal autoHeal, ref RecivedDamage recivedDamage)
            {
                autoHeal.HealLoadPercent = recivedDamage.Value > 0 ? 0 :
                    Maths.MoveTowards(autoHeal.HealLoadPercent, 1, _deltaTime / autoHeal.HealDelay);

                health.Value = autoHeal.HealLoadPercent != 1 ? health.Value :
                    Maths.MoveTowards(health.Value, maxHealth.Value, autoHeal.HealPerSecond * _deltaTime);
            }
        }
    }
}