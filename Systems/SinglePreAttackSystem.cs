using Arch.Core;
using DVG.SkyPirates.Shared.Components.Config;
using DVG.SkyPirates.Shared.Components.Framed;
using DVG.SkyPirates.Shared.Components.Runtime;
using DVG.SkyPirates.Shared.Ids;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;

namespace DVG.SkyPirates.Shared.Systems
{
    /// <summary>
    /// Switches <see cref="BehaviourState"/> to PreAttack if State is None and Target is in ImpactDistance
    /// </summary>
    public sealed class SinglePreAttackSystem : ITickableExecutor
    {
        private readonly QueryDescription _desc = new QueryDescription().
            WithAll<BehaviourState, ImpactDistance, Position, Target>().NotDisposing();

        private readonly World _world;

        public SinglePreAttackSystem(World world)
        {
            _world = world;
        }

        public void Tick(int tick, fix deltaTime)
        {
            var query = new PreAttackQuery(_world);
            _world.InlineQuery<PreAttackQuery, BehaviourState, ImpactDistance, Position, Target>(_desc, ref query);
        }

        private readonly struct PreAttackQuery :
            IForEach<BehaviourState, ImpactDistance, Position, Target>
        {
            private readonly World _world;

            public PreAttackQuery(World world)
            {
                _world = world;
            }

            public void Update(ref BehaviourState behaviour, ref ImpactDistance impactDistance, ref Position position, ref Target target)
            {
                if (behaviour.State != StateId.None)
                    return;

                if (!target.Entity.HasValue)
                    return;

                var targetPos = _world.Get<Position>(target.Entity.Value);
                var sqrDistance = fix3.SqrDistance(targetPos, position);
                var impactSqrDistance = (fix)impactDistance * impactDistance;

                if (sqrDistance > impactSqrDistance)
                    return;

                behaviour.ForceState = StateId.Constants.PreAttack;
            }
        }
    }
}
