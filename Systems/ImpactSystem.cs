using Arch.Core;
using Arch.Core.Extensions;
using DVG.SkyPirates.Shared.Components;
using DVG.SkyPirates.Shared.Components.Data;
using DVG.SkyPirates.Shared.Ids;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;

namespace DVG.SkyPirates.Shared.Systems
{
    public class ImpactSystem : ITickableExecutor
    {
        private readonly QueryDescription _desc = new QueryDescription().
            WithAll<Behaviour, Damage, ImpactDistance, Position, Target>().
            WithNone<Dead>();

        private readonly World _world;

        public ImpactSystem(World world)
        {
            _world = world;
        }

        public void Tick(int tick, fix deltaTime)
        {
            var query = new ImpactQuery();
            _world.InlineQuery<ImpactQuery, Behaviour, Damage, ImpactDistance, Position, Target>(_desc, ref query);
        }

        private readonly struct ImpactQuery :
            IForEach<Behaviour, Damage, ImpactDistance, Position, Target>
        {
            public void Update(ref Behaviour behaviour, ref Damage damage, ref ImpactDistance impactDistance, ref Position position, ref Target target)
            {
                if (behaviour.State != StateId.Constants.PreAttack || behaviour.Percent != 1)
                    return;

                if (!target.Entity.IsAlive())
                    return;

                var sqrDistance = fix3.SqrDistance(target.Entity.Get<Position>().Value, position.Value);
                var impactSqrDistance = impactDistance.Value * impactDistance.Value;

                if (sqrDistance > impactSqrDistance)
                    return;

                target.Entity.Get<RecivedDamage>().Value += damage.Value;
            }
        }
    }
}
