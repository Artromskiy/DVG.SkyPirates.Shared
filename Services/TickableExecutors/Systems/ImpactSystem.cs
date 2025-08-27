using Arch.Core;
using Arch.Core.Extensions;
using DVG.SkyPirates.Shared.Components;
using DVG.SkyPirates.Shared.Components.Data;
using DVG.SkyPirates.Shared.Ids;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;

namespace DVG.SkyPirates.Shared.Services.TickableExecutors.Systems
{
    public class ImpactSystem : ITickableExecutor
    {
        private readonly QueryDescription _desc = new QueryDescription().WithAll<Behaviour, Damage, ImpactDistance, Position, Target>();
        private readonly World _world;

        public ImpactSystem(World world)
        {
            _world = world;
        }

        public void Tick(int tick, fix deltaTime)
        {
            _world.InlineQuery<AttackQuery, Behaviour, Damage, ImpactDistance, Position, Target>(_desc);
        }

        private readonly struct AttackQuery :
            IForEach<Behaviour, Damage, ImpactDistance, Position, Target>
        {
            public void Update(ref Behaviour behaviour, ref Damage damage, ref ImpactDistance impactDistance, ref Position position, ref Target target)
            {
                if (behaviour.State != StateId.Constants.PreAttack || behaviour.Percent != 1)
                    return;

                if (!target.Entity.IsAlive())
                    return;

                var sqrDistance = fix3.SqrDistance(target.Entity.Get<Position>().Value, position.Value);
                if (sqrDistance < impactDistance.Value * impactDistance.Value)
                    return;

                target.Entity.Get<Health>().Value -= damage.Value;
            }
        }
    }
}
