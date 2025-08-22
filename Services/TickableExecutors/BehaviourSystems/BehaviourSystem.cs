using Arch.Core;
using Arch.Core.Extensions;
using DVG.Core;
using DVG.SkyPirates.Shared.Components;
using DVG.SkyPirates.Shared.Ids;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Services.TickableExecutors.BehaviourSystems
{
    public class BehaviourSystem : ITickableExecutor
    {
        private readonly World _world;
        private readonly List<(Entity, StateId, StateId)> _stateSwitchCache = new List<(Entity, StateId, StateId)>();
        public BehaviourSystem(World world)
        {
            _world = world;
        }

        public void Tick(int tick, fix deltaTime)
        {
            _stateSwitchCache.Clear();
            var switchQuery = new SwitchBehaviourQuery(_stateSwitchCache);
            _world.InlineEntityQuery<SwitchBehaviourQuery, Behaviour, BehaviourConfig>
                (new QueryDescription().WithAll<Behaviour>(), ref switchQuery);

            foreach (var (entity, prevState, newState) in _stateSwitchCache)
            {
                prevState.GenericCall(new RemoveState(entity));
                newState.GenericCall(new AddState(entity));
            }

            var tickQuery = new TickBehaviourQuery(deltaTime);
            _world.InlineQuery<TickBehaviourQuery, Behaviour>
                (new QueryDescription().WithAll<Behaviour>(), ref tickQuery);
        }

        private readonly struct RemoveState : IGenericAction<StateId.IFlag>
        {
            private readonly Entity _entity;

            public RemoveState(Entity entity)
            {
                _entity = entity;
            }

            public void Invoke<T>() where T : StateId.IFlag
            {
                _entity.Remove<StateId<T>>();
            }
        }

        private readonly struct AddState : IGenericAction<StateId.IFlag>
        {
            private readonly Entity _entity;

            public AddState(Entity entity)
            {
                this._entity = entity;
            }

            public void Invoke<T>() where T : StateId.IFlag
            {
                _entity.Add<StateId<T>>();
            }
        }

        private readonly struct SwitchBehaviourQuery : IForEachWithEntity<Behaviour, BehaviourConfig>
        {
            private readonly List<(Entity entity, StateId from, StateId to)> _stateSwitch;

            public SwitchBehaviourQuery(List<(Entity entity, StateId from, StateId to)> stateSwitch)
            {
                _stateSwitch = stateSwitch;
            }

            public void Update(Entity entity, ref Behaviour behaviour, ref BehaviourConfig behaviourConfig)
            {
                if (behaviour.Percent != 1)
                    return;

                var prevState = behaviour.State;
                behaviour.State = behaviourConfig.Scenario[behaviour.State];
                behaviour.Duration = behaviourConfig.Durations[behaviour.State];
                behaviour.Percent = 0;

                _stateSwitch.Add((entity, prevState, behaviour.State));
            }
        }

        private readonly struct TickBehaviourQuery : IForEach<Behaviour>
        {
            private readonly fix _deltaTime;

            public TickBehaviourQuery(fix deltaTime)
            {
                _deltaTime = deltaTime;
            }

            public void Update(ref Behaviour behaviour)
            {
                behaviour.Percent = Maths.MoveTowards(behaviour.Percent, 1, _deltaTime / behaviour.Duration);
            }
        }
    }
}