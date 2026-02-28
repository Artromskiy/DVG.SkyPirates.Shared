using Arch.Core;
using DVG.SkyPirates.Shared.Components.Config;
using DVG.SkyPirates.Shared.Components.Framed;
using DVG.SkyPirates.Shared.Components.Runtime;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;

namespace DVG.SkyPirates.Shared.Systems
{
    public sealed class AutoHealSystem : ITickableExecutor
    {
        private readonly QueryDescription _healLoadDesc = new QueryDescription().
            WithAll<Health, MaxHealth, AutoHeal, RecivedDamage>().NotDisposing().NotDisabled();

        private readonly QueryDescription _healDesc = new QueryDescription().
            WithAll<Health, MaxHealth, AutoHeal>().NotDisposing().NotDisabled();

        private readonly World _world;

        public AutoHealSystem(World world)
        {
            _world = world;
        }

        public void Tick(int tick, fix deltaTime)
        {
            var healLoadQuery = new HealLoadQuery(deltaTime);
            _world.InlineQuery<HealLoadQuery, AutoHeal, RecivedDamage>(_healLoadDesc, ref healLoadQuery);

            var healQuery = new HealQuery(deltaTime);
            _world.InlineQuery<HealQuery, Health, MaxHealth, AutoHeal>(_healDesc, ref healQuery);
        }

        private readonly struct HealLoadQuery :
            IForEach<AutoHeal, RecivedDamage>
        {
            private readonly fix _deltaTime;

            public HealLoadQuery(fix deltaTime)
            {
                _deltaTime = deltaTime;
            }

            public void Update(ref AutoHeal autoHeal, ref RecivedDamage recivedDamage)
            {
                autoHeal.HealLoadPercent = recivedDamage > fix.Zero ? 0 :
                    Maths.MoveTowards(autoHeal.HealLoadPercent, 1, _deltaTime / autoHeal.HealDelay);
            }
        }

        private readonly struct HealQuery : IForEach<Health, MaxHealth, AutoHeal>
        {
            private readonly fix _deltaTime;

            public HealQuery(fix deltaTime)
            {
                _deltaTime = deltaTime;
            }

            public void Update(ref Health health, ref MaxHealth maxHealth, ref AutoHeal autoHeal)
            {
                health = autoHeal.HealLoadPercent != 1 ? health :
                    Maths.MoveTowards(health, maxHealth, autoHeal.HealPerSecond * _deltaTime);
            }
        }
    }
}