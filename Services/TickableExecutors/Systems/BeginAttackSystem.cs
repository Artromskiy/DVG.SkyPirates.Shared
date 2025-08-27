using Arch.Core;
using Arch.Core.Extensions;
using DVG.SkyPirates.Shared.Components;
using DVG.SkyPirates.Shared.Components.Data;
using DVG.SkyPirates.Shared.Ids;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;

namespace DVG.SkyPirates.Shared.Services.TickableExecutors.Systems
{
    /// <summary>
    /// Switches <see cref="Behaviour"/> to PreAttack if State is None and Target is in ImpactDistance
    /// </summary>
    public class BeginAttackSystem : ITickableExecutor
    {
        private readonly QueryDescription _desc = new QueryDescription().WithAll<Behaviour, ImpactDistance, Position, Target>();
        private readonly World _world;

        public BeginAttackSystem(World world)
        {
            _world = world;
        }

        public void Tick(int tick, fix deltaTime)
        {
            _world.InlineQuery<BeginAttackQuery, Behaviour, ImpactDistance, Position, Target>(_desc);
        }

        private readonly struct BeginAttackQuery :
            IForEach<Behaviour, ImpactDistance, Position, Target>
        {
            public void Update(ref Behaviour behaviour, ref ImpactDistance impactDistance, ref Position position, ref Target target)
            {
                if (behaviour.State == StateId.None)
                    return;

                if (!target.Entity.IsAlive())
                    return;

                var sqrDistance = fix3.SqrDistance(target.Entity.Get<Position>().Value, position.Value);
                if (sqrDistance < impactDistance.Value * impactDistance.Value)
                    return;

                behaviour.ForceState = StateId.Constants.PreAttack;
            }
        }
    }
}
