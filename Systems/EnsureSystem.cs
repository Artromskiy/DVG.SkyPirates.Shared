using Arch.Core;
using DVG.SkyPirates.Shared.Components;
using DVG.SkyPirates.Shared.Components.Config;
using DVG.SkyPirates.Shared.Components.Special;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;

namespace DVG.SkyPirates.Shared.Systems
{
    public class EnsureSystem : ITickableExecutor
    {
        private readonly World _world;
        private readonly QueryDescription _separationForceDesc = new QueryDescription().WithAll<Separation>().WithNone<SeparationForce>();
        private readonly QueryDescription _autoHealDesc = new QueryDescription().WithAll<MaxHealth>().WithNone<AutoHeal>();
        private readonly QueryDescription _behaviourStateDesc = new QueryDescription().WithAll<BehaviourConfig>().WithNone<BehaviourState>();

        private readonly AutoHeal _defaultAutoHeal = new()
        {
            HealDelay = 10,
            HealPerSecond = 20
        };

        public EnsureSystem(World world)
        {
            _world = world;
        }

        public void Tick(int tick, fix deltaTime)
        {
            _world.AddQuery((ref MaxHealth maxHealth, ref Health health) => health.Value = maxHealth.Value);
            _world.Add(_separationForceDesc, new SeparationForce());
            _world.Add(_autoHealDesc, _defaultAutoHeal);
            _world.Add(_behaviourStateDesc, new BehaviourState());
        }
    }
}
