using Arch.Core;
using Arch.Core.Extensions;
using DVG.SkyPirates.Shared.Components;
using DVG.SkyPirates.Shared.Ids;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;

namespace DVG.SkyPirates.Shared.Services.TickableExecutors.BehaviourSystems
{
    public class ImpactSystem : ITickableExecutor
    {
        private readonly World _world;
        public ImpactSystem(World world)
        {
            _world = world;
        }

        public void Tick(int tick, fix deltaTime)
        {
            var desc = new QueryDescription().WithAll<Behaviour, Position, ImpactDistance, Damage, Target>();
            _world.InlineQuery<ImpactQuery, Behaviour, Position, ImpactDistance, Damage, Target >
                (desc);
        }

        private readonly struct ImpactQuery :
            IForEach<Behaviour, Position, ImpactDistance, Damage, Target>
        {
            public void Update(ref Behaviour behaviour, ref Position position, ref ImpactDistance impactDistance, ref Damage damage, ref Target target)
            {
                if (behaviour.State != StateId.Constants.Impact)
                    return;

                Impact(ref position.Value, damage.Value, impactDistance.Value, target.Entity);
            }

            private static void Impact(
                ref fix3 position,
                fix damage,
                fix impactDistance,
                Entity target)
            {
                var impactSqrDistance = impactDistance * impactDistance;
                var targetPosition = target.Get<Position>().Value;
                var sqrDistance = fix3.SqrDistance(position, targetPosition);
                if (sqrDistance <= impactSqrDistance)
                    target.Get<Health>().Value -= damage;
            }
        }
    }
}
