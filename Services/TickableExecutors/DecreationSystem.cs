using Arch.Core;
using Arch.Core.Extensions;
using DVG.Core;
using DVG.SkyPirates.Shared.Archetypes;
using DVG.SkyPirates.Shared.Components;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Services.TickableExecutors
{
    public class DecreationSystem : IPreTickableExecutor
    {
        private readonly World _world;
        private readonly List<Entity> _notCreatedEntities = new List<Entity>();

        public DecreationSystem(World world)
        {
            _world = world;
        }

        public void Tick(int tick, fix deltaTime)
        {
            _notCreatedEntities.Clear();
            var query = new SelectNotCreatedQuery(_notCreatedEntities, tick);
            var desc = new QueryDescription().WithAll<Creation>();
            _world.InlineEntityQuery<SelectNotCreatedQuery, Creation>(desc, ref query);

            foreach (var entity in _notCreatedEntities)
                HistoryArch.ForEachData(new RemoveHistoryAction(entity));
        }

        private readonly struct SelectNotCreatedQuery : IForEachWithEntity<Creation>
        {
            private readonly List<Entity> _notCreatedEntities;
            private readonly int _tick;

            public SelectNotCreatedQuery(List<Entity> notCreatedEntities, int tick)
            {
                _notCreatedEntities = notCreatedEntities;
                _tick = tick;
            }

            public readonly void Update(Entity entity, ref Creation creation)
            {
                if (_tick < creation.Tick)
                    _notCreatedEntities.Add(entity);
            }
        }

        private readonly struct RemoveHistoryAction : IGenericAction
        {
            private readonly Entity _entity;

            public RemoveHistoryAction(Entity entity)
            {
                _entity = entity;
            }

            public void Invoke<T>()
            {
                if(_entity.Has<T>())
                    _entity.Remove<T>();
            }
        }
    }
}
