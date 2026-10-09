using System.Collections.Generic;
using Delta.ECS;
using DVG.Components;
using DVG.SkyPirates.Shared.Data;
using DVG.SkyPirates.Shared.Ecs;
using DVG.SkyPirates.Shared.IFactories;
using DVG.SkyPirates.Shared.IServices;

namespace DVG.SkyPirates.Shared.Systems.Special
{
    internal sealed class SnapshotHistorySystem
    {
        private readonly World _world;
        private readonly WorldHistory _history;
        private readonly ComponentId _aliveId;
        private readonly IEntityFactory _entityFactory;
        private readonly IEntityRegistry _entityRegistry;

        public SnapshotHistorySystem(World world, WorldHistory history, IEntityFactory entityFactory, IEntityRegistry entityRegistry)
        {
            _world = world;
            _history = history;
            _aliveId = world.Layouts.GetPrimary<Alive>();
            _entityFactory = entityFactory;
            _entityRegistry = entityRegistry;
        }

        public void ApplySnapshot(WorldData snapshot)
        {
            foreach (var syncId in snapshot.Get<SyncId>().Values)
                _entityRegistry.Reserve(syncId);
            foreach (var syncIdReserve in snapshot.Get<SyncIdReserve>().Values)
                _entityRegistry.Reserve(syncIdReserve);

            var alive = snapshot.Get<Alive>();
            var entitiesBySyncId = new Dictionary<int, Entity>();
            foreach (var syncId in snapshot.Get<SyncId>().Values)
            {
                Entity entity = _entityFactory.Create(new()
                {
                    SyncId = syncId,
                    RandomSeed = default,
                    SyncIdReserve = default,
                });

                entitiesBySyncId.Add(syncId.Value, entity);
                if (!alive.ContainsKey(syncId.Value))
                    _world.Remove(entity, _aliveId);
            }

            _history.ApplySnapshot(snapshot, entitiesBySyncId);
        }
    }
}
