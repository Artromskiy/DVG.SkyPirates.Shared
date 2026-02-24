using Arch.Core;
using DVG.SkyPirates.Shared.Components.Config;
using DVG.SkyPirates.Shared.Components.Runtime;
using DVG.SkyPirates.Shared.Ids;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;

namespace DVG.SkyPirates.Shared.Systems
{
    public sealed class SimpleBehaviourSystem : ITickableExecutor
    {
        private readonly QueryDescription _descSwitch = new QueryDescription().
            WithAll<BehaviourState, BehaviourConfig>().NotDisposing().NotDisabled();

        private readonly QueryDescription _descTick = new QueryDescription().
            WithAll<BehaviourState>().NotDisposing().NotDisabled();

        private readonly World _world;

        public SimpleBehaviourSystem(World world)
        {
            _world = world;
        }

        public void Tick(int tick, fix deltaTime)
        {
            var query = new BehaviourQuery(deltaTime);
            _world.InlineQuery<BehaviourQuery, BehaviourState, BehaviourConfig>(_descSwitch, ref query);
            _world.InlineQuery<BehaviourQuery, BehaviourState>(_descTick, ref query);

        }

        private readonly struct BehaviourQuery :
            IForEach<BehaviourState, BehaviourConfig>,
            IForEach<BehaviourState>
        {
            private readonly fix _deltaTime;

            public BehaviourQuery(fix deltaTime)
            {
                _deltaTime = deltaTime;
            }

            public void Update(ref BehaviourState behaviour, ref BehaviourConfig behaviourConfig)
            {
                // skip if no force state and we are at none
                if (behaviour.ForceState == null && (
                    behaviour.Percent != 1 || behaviour.State.IsNone))
                    return;

                StateId targetState = behaviour.ForceState ??=
                    behaviourConfig.Scenario[behaviour.State];

                behaviour.ForceState = null;
                behaviour.State = targetState;
                behaviour.Duration = behaviourConfig.Durations[behaviour.State];
                behaviour.Percent = 0;
            }

            public void Update(ref BehaviourState behaviour)
            {
                fix step = behaviour.Duration == 0 ? 1 : _deltaTime / behaviour.Duration;
                behaviour.Percent = Maths.MoveTowards(behaviour.Percent, 1, step);
            }
        }
    }
}