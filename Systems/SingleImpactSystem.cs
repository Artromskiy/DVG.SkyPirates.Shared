using Arch.Core;
using DVG.SkyPirates.Shared.Components;
using DVG.SkyPirates.Shared.Components.Data;
using DVG.SkyPirates.Shared.Ids;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;

namespace DVG.SkyPirates.Shared.Systems
{
    public sealed class SingleImpactSystem : ITickableExecutor
    {
        private readonly QueryDescription _desc = new QueryDescription().
            WithAll<BehaviourState, Damage, ImpactDistance, Position, Target, Alive>();

        private readonly World _world;

        public SingleImpactSystem(World world)
        {
            _world = world;
        }

        public void Tick(int tick, fix deltaTime)
        {
            var query = new ImpactQuery(_world);
            _world.InlineQuery<ImpactQuery, BehaviourState, Damage, ImpactDistance, Position, Target>(_desc, ref query);
        }

        private readonly struct ImpactQuery :
            IForEach<BehaviourState, Damage, ImpactDistance, Position, Target>
        {
            private readonly World _world;

            public ImpactQuery(World world)
            {
                _world = world;
            }

            public void Update(ref BehaviourState behaviour, ref Damage damage, ref ImpactDistance impactDistance, ref Position position, ref Target target)
            {
                if (behaviour.State != StateId.Constants.PreAttack || behaviour.Percent != 1)
                    return;

                if (!target.Entity.HasValue)
                    return;

                var targetPos = _world.Get<Position>(target.Entity.Value);
                var sqrDistance = fix3.SqrDistance(targetPos.Value, position.Value);
                var impactSqrDistance = impactDistance.Value * impactDistance.Value;

                if (sqrDistance > impactSqrDistance)
                    return;

                _world.Get<RecivedDamage>(target.Entity.Value).Value += damage.Value;
            }
        }
    }
}
