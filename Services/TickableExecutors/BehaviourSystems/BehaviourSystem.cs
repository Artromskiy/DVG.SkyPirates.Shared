using Arch.Core;
using DVG.SkyPirates.Shared.Components;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;

namespace DVG.SkyPirates.Shared.Services.TickableExecutors.BehaviourSystems
{
    public class BehaviourSystem : ITickableExecutor
    {
        private readonly World _world;
        public BehaviourSystem(World world)
        {
            _world = world;
        }

        public void Tick(int tick, fix deltaTime)
        {
            var query = new BehaviourQuery(deltaTime);
            _world.InlineQuery<BehaviourQuery, Behaviour, BehaviourConfig>
                (new QueryDescription().WithAll<Behaviour>(), ref query);
        }

        private readonly struct BehaviourQuery : IForEach<Behaviour, BehaviourConfig>
        {
            private readonly fix _deltaTime;

            public BehaviourQuery(fix deltaTime)
            {
                _deltaTime = deltaTime;
            }

            public void Update(ref Behaviour behaviour, ref BehaviourConfig behaviourConfig)
            {
                if(behaviour.Percent == 1)
                {
                    behaviour.State = behaviourConfig.Scenario[behaviour.State];
                    behaviour.Duration = behaviourConfig.Durations[behaviour.State];
                    behaviour.Percent = 0;
                }
                behaviour.Percent = Maths.MoveTowards(behaviour.Percent, 1, _deltaTime / behaviour.Duration);
            }
        }
    }
}