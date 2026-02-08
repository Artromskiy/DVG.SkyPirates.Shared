using Arch.Core;
using DVG.SkyPirates.Shared.Components.Config;
using DVG.SkyPirates.Shared.Components.Framed;
using DVG.SkyPirates.Shared.Components.Runtime;
using DVG.SkyPirates.Shared.Ids;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;
using DVG.SkyPirates.Shared.Systems.Special;

namespace DVG.SkyPirates.Shared.Systems
{
    public class MultiPreAttackSystem : ITickableExecutor
    {
        private readonly QueryDescription _desc = new QueryDescription().
            WithAll<BehaviourState, ImpactDistance, Position, Targets>().NotDisposing();

        private readonly World _world;

        public MultiPreAttackSystem(World world)
        {
            _world = world;
        }

        public void Tick(int tick, fix deltaTime)
        {
            var query = new PreAttackQuery(_world);
            _world.InlineQuery<PreAttackQuery, BehaviourState, ImpactDistance, Position, Targets>(_desc, ref query);
        }

        private readonly struct PreAttackQuery :
            IForEach<BehaviourState, ImpactDistance, Position, Targets>
        {
            private readonly World _world;

            public PreAttackQuery(World world)
            {
                _world = world;
            }

            public void Update(ref BehaviourState behaviour, ref ImpactDistance impactDistance, ref Position position, ref Targets targets)
            {
                if (behaviour.State != StateId.None)
                    return;

                if (targets.Entities == null || targets.Entities.Count == 0)
                    return;

                var impactSqrDistance = impactDistance.Value * impactDistance.Value;

                for (int i = 0; i < targets.Entities.Count; i++)
                {
                    var targetPos = _world.Get<Position>(targets.Entities[i]);
                    var sqrDistance = fix3.SqrDistance(targetPos.Value, position.Value);
                    if (sqrDistance <= impactSqrDistance)
                    {
                        behaviour.ForceState = StateId.Constants.PreAttack;
                        return;
                    }
                }
            }
        }
    }
}
