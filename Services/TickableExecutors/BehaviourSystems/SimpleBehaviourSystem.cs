using Arch.Core;
using DVG.SkyPirates.Shared.Components;
using DVG.SkyPirates.Shared.Ids;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;

namespace DVG.SkyPirates.Shared.Services.TickableExecutors.BehaviourSystems
{
    public class SimpleBehaviourSystem : ITickableExecutor
    {
        private readonly World _world;

        public SimpleBehaviourSystem(World world)
        {
            _world = world;
        }

        public void Tick(int tick, fix deltaTime)
        {
            var descSwitch = new QueryDescription().WithAll<Behaviour, BehaviourConfig>();
            var descTick = new QueryDescription().WithAll<Behaviour>();
            var query = new BehaviourQuery(tick);
            _world.InlineQuery<BehaviourQuery, Behaviour, BehaviourConfig>(descSwitch, ref query);
            _world.InlineQuery<BehaviourQuery, Behaviour>(descTick, ref query);

        }

        private readonly struct BehaviourQuery :
            IForEach<Behaviour, BehaviourConfig>,
            IForEach<Behaviour>
        {
            private readonly fix _deltaTime;

            public BehaviourQuery(fix deltaTime)
            {
                _deltaTime = deltaTime;
            }

            public void Update(ref Behaviour behaviour, ref BehaviourConfig behaviourConfig)
            {
                if (behaviour.Percent != 1 || behaviour.ForceState == null)
                    return;

                StateId targetState = behaviour.ForceState ??= behaviour.State;

                behaviour.ForceState = null;
                behaviour.State = behaviourConfig.Scenario[targetState];
                behaviour.Duration = behaviourConfig.Durations[behaviour.State];
                behaviour.Percent = 0;
            }

            public void Update(ref Behaviour behaviour)
            {
                fix step = behaviour.Duration == 0 ? 1 : _deltaTime / behaviour.Duration;
                behaviour.Percent = Maths.MoveTowards(behaviour.Percent, 1, step);
            }
        }
    }
}