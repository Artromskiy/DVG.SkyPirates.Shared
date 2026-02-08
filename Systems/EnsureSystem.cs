using Arch.Core;
using DVG.SkyPirates.Shared.Components.Config;
using DVG.SkyPirates.Shared.Components.Runtime;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;
using DVG.SkyPirates.Shared.Systems.Special;

namespace DVG.SkyPirates.Shared.Systems
{
    public class EnsureSystem : ITickableExecutor
    {
        private readonly World _world;
        //private readonly QueryDescription _cachePositionDesc = new QueryDescription().WithAll<Position>().WithNone<CachePosition>();
        //private readonly QueryDescription _separationForceDesc = new QueryDescription().WithAll<Separation>().WithNone<SeparationForce>();
        //private readonly QueryDescription _autoHealDesc = new QueryDescription().WithAll<MaxHealth>().WithNone<AutoHeal>();
        //private readonly QueryDescription _recievedDamageDesc = new QueryDescription().WithAll<MaxHealth>().WithNone<RecivedDamage>();
        //private readonly QueryDescription _behaviourStateDesc = new QueryDescription().WithAll<BehaviourConfig>().WithNone<BehaviourState>();
        //private readonly QueryDescription _squadMembersCountDesc = new QueryDescription().WithAll<Squad>().WithNone<SquadMemberCount>();

        //private readonly AutoHeal _defaultAutoHeal = new()
        //{
        //    HealDelay = 10,
        //    HealPerSecond = 20
        //};

        public EnsureSystem(World world)
        {
            _world = world;
        }

        public void Tick(int tick, fix deltaTime)
        {
            EnsureRuntime();
            EnsureFramed();
        }

        private void EnsureRuntime()
        {
            _world.AddQuery((ref MaxHealth maxHealth, ref Health health) => health.Value = maxHealth.Value);
            //_world.Add(in _autoHealDesc, _defaultAutoHeal);
            //_world.Add(in _behaviourStateDesc, new BehaviourState());
        }

        private void EnsureFramed()
        {
            //_world.Add(in _squadMembersCountDesc, new SquadMemberCount());
            //_world.Add(in _recievedDamageDesc, new RecivedDamage());
            //_world.Add(in _cachePositionDesc, new CachePosition());
            //_world.Add(in _separationForceDesc, new SeparationForce());
        }
    }
}
