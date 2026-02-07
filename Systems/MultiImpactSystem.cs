using Arch.Core;
using DVG.SkyPirates.Shared.Components.Config;
using DVG.SkyPirates.Shared.Components.Framed;
using DVG.SkyPirates.Shared.Components.Runtime;
using DVG.SkyPirates.Shared.Ids;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;

namespace DVG.SkyPirates.Shared.Systems
{
    public sealed class MultiImpactSystem : ITickableExecutor
    {
        private readonly QueryDescription _desc = new QueryDescription().
            WithAll<BehaviourState, Damage, ImpactDistance, Position, Targets>();

        private readonly World _world;

        public MultiImpactSystem(World world)
        {
            _world = world;
        }

        public void Tick(int tick, fix deltaTime)
        {
            var query = new ImpactQuery(_world);
            _world.InlineQuery<ImpactQuery, BehaviourState, Damage, ImpactDistance, Position, Targets>(_desc, ref query);
        }

        private readonly struct ImpactQuery :
            IForEach<BehaviourState, Damage, ImpactDistance, Position, Targets>
        {
            private readonly World _world;

            public ImpactQuery(World world)
            {
                _world = world;
            }

            public void Update(ref BehaviourState behaviour, ref Damage damage, ref ImpactDistance impactDistance, ref Position position, ref Targets targets)
            {
                if (behaviour.State != StateId.Constants.PreAttack || behaviour.Percent != 1)
                    return;

                if (targets.Entities == null || targets.Entities.Count == 0)
                    return;

                var impactSqrDistance = impactDistance.Value * impactDistance.Value;
                for (int i = 0; i < targets.Entities.Count; i++)
                {

                    var targetPos = _world.Get<Position>(targets.Entities[i]);
                    var sqrDistance = fix3.SqrDistance(targetPos.Value, position.Value);

                    if (sqrDistance > impactSqrDistance)
                        continue;

                    _world.Get<RecivedDamage>(targets.Entities[i]).Value += damage.Value;
                }
            }
        }
    }
}
