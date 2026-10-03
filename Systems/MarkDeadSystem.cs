using Delta;
using Delta.ECS;
using DVG.SkyPirates.Shared.Ecs;
using DVG.Components;
using DVG.SkyPirates.Shared.Components.Runtime;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;
using System;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Systems
{
    [Obsolete]
    public sealed class MarkDeadSystem : IDeltaTickableExecutor
    {
        private Query? _descCache;
        private Query _desc => _descCache ??= _world.WhereAll<Health>().
            Alive().NotDisabled();

        private readonly List<Entity> _dead = new();

        private readonly World _world;
        public MarkDeadSystem(World world)
        {
            _world = world;
        }

        public void Tick(int tick, fix deltaTime)
        {
            _dead.Clear();
            var dead = _dead;
            var desc = _desc;
            _world.ForEachEntity<List<Entity>, Health>(in desc, ref dead,
                static (ref List<Entity> entities, Entity entity, ref Health health) =>
                {
                    if (health <= fix.Zero)
                        entities.Add(entity);
                });
            foreach (var item in _dead)
            {
                _world.Remove<Alive>(item);
            }
        }
    }
}
