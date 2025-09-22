using Arch.Core;
using DVG.SkyPirates.Shared.Components;
using DVG.SkyPirates.Shared.Components.Data;
using DVG.SkyPirates.Shared.Ids;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;

namespace DVG.SkyPirates.Shared.Systems
{
    /// <summary>
    /// Switches <see cref="Behaviour"/> to PreAttack if State is None and Target is in ImpactDistance
    /// </summary>
    public sealed class BeginAttackSystem : ITickableExecutor
    {
        private readonly QueryDescription _desc = new QueryDescription().
            WithAll<Behaviour, ImpactDistance, Position, Target, Alive>();

        private readonly World _world;

        public BeginAttackSystem(World world)
        {
            _world = world;
        }

        public void Tick(int tick, fix deltaTime)
        {
            var query = new BeginAttackQuery(_world);
            _world.InlineQuery<BeginAttackQuery, Behaviour, ImpactDistance, Position, Target>(_desc, ref query);
        }

        private readonly struct BeginAttackQuery :
            IForEach<Behaviour, ImpactDistance, Position, Target>
        {
            private readonly World _world;

            public BeginAttackQuery(World world)
            {
                _world = world;
            }

            public void Update(ref Behaviour behaviour, ref ImpactDistance impactDistance, ref Position position, ref Target target)
            {
                if (behaviour.State != StateId.None)
                    return;

                if (!target.Entity.HasValue)
                    return;

                var targetPos = _world.Get<Position>(target.Entity.Value);
                var sqrDistance = fix3.SqrDistance(targetPos.Value, position.Value);
                var impactSqrDistance = impactDistance.Value * impactDistance.Value;

                if (sqrDistance > impactSqrDistance)
                    return;

                behaviour.ForceState = StateId.Constants.PreAttack;
            }
        }
    }
}
