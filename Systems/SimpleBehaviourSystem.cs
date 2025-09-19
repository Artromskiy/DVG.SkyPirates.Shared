using Arch.Core;
using DVG.SkyPirates.Shared.Components;
using DVG.SkyPirates.Shared.Ids;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;

namespace DVG.SkyPirates.Shared.Systems
{
    public sealed class SimpleBehaviourSystem : ITickableExecutor
    {
        private readonly QueryDescription _descSwitch = new QueryDescription().
            WithAll<Behaviour, BehaviourConfig>().
            WithNone<Dead>();

        private readonly QueryDescription _descTick = new QueryDescription().
            WithAll<Behaviour>().
            WithNone<Dead>();

        private readonly World _world;

        public SimpleBehaviourSystem(World world)
        {
            _world = world;
        }

        public void Tick(int tick, fix deltaTime)
        {
            var query = new BehaviourQuery(deltaTime);
            _world.InlineQuery<BehaviourQuery, Behaviour, BehaviourConfig>(_descSwitch, ref query);
            _world.InlineQuery<BehaviourQuery, Behaviour>(_descTick, ref query);

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
                if (behaviour.Percent != 1 && behaviour.ForceState == null)
                    return;

                StateId targetState = behaviour.ForceState ??=
                    behaviourConfig.Scenario[behaviour.State];

                behaviour.ForceState = null;
                behaviour.State = targetState;
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