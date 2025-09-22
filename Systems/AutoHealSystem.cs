using Arch.Core;
using DVG.SkyPirates.Shared.Components;
using DVG.SkyPirates.Shared.Components.Data;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;

namespace DVG.SkyPirates.Shared.Systems
{
    public sealed class AutoHealSystem : ITickableExecutor
    {
        private readonly QueryDescription _desc = new QueryDescription().
            WithAll<Health, MaxHealth, AutoHeal, RecivedDamage, Alive>();

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
                autoHeal.healLoadPercent = recivedDamage.Value > 0 ? 0 :
                    Maths.MoveTowards(autoHeal.healLoadPercent, 1, _deltaTime / autoHeal.healDelay);

                health.Value = autoHeal.healLoadPercent != 1 ? health.Value :
                    Maths.MoveTowards(health.Value, maxHealth.Value, autoHeal.healPerSecond * _deltaTime);
            }
        }
    }
}