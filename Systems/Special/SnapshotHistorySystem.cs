using Arch.Core;
using DVG.Collections;
using DVG.Components;
using DVG.SkyPirates.Shared.Data;
using DVG.SkyPirates.Shared.IFactories;
using DVG.SkyPirates.Shared.IServices;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Systems.Special
{
    public class SnapshotHistorySystem
    {
        private class Description<T> where T : struct
        {
            public QueryDescription Desc = new QueryDescription().WithAll<History<T>>();
        }
        private readonly GenericCreator _desc = new();

        private readonly World _world;
        private readonly IEntityFactory _entityFactory;
        private readonly IEntityRegistry _entityRegistry;

        public SnapshotHistorySystem(World world, IEntityFactory entityFactory, IEntityRegistry entityRegistry)
        {
            _world = world;
            _entityFactory = entityFactory;
            _entityRegistry = entityRegistry;
        }

        public WorldData GetSnapshot(int tick)
        {
            var worldData = new WorldData();
            var getAction = new GetAction(_world, worldData, _desc, tick);
            HistoryComponentsRegistry.ForEachData(ref getAction);
            return worldData;
        }

        public void ApplySnapshot(WorldData snapshot)
        {
            foreach (var syncId in snapshot.Get<SyncId>().Values)
                _entityRegistry.Reserve(syncId);

            foreach (var syncIdReserve in snapshot.Get<SyncIdReserve>().Values)
                _entityRegistry.Reserve(syncIdReserve);

            foreach (var syncId in snapshot.Get<SyncId>().Values)
            {
                _entityFactory.Create(new()
                {
                    SyncId = syncId,
                    RandomSeed = default,
                    SyncIdReserve = default,
                });
            }
            var unpackAction = new ApplyAction(_entityRegistry, snapshot, _world);
            HistoryComponentsRegistry.ForEachData(ref unpackAction);
            _world.TrimExcess();
        }

        private readonly struct GetAction : IStructGenericAction
        {
            private readonly GenericCreator _desc;
            private readonly WorldData _worldData;
            private readonly World _world;
            private readonly int _tick;

            public GetAction(World world, WorldData entities, GenericCreator desc, int tick)
            {
                _world = world;
                _worldData = entities;
                _desc = desc;
                _tick = tick;
            }

            public readonly void Invoke<T>() where T : struct
            {
                var components = _worldData.Get<T>();
                var query = new PackQuery<T>(components, _tick);
                var desc = _desc.Get<Description<T>>().Desc;
                _world.InlineQuery<PackQuery<T>, History<T>, History<SyncId>>(desc, ref query);
            }

            private readonly struct PackQuery<T> : IForEach<History<T>, History<SyncId>> where T : struct
            {
                private readonly Dictionary<int, T> _components;
                private readonly int _tick;

                public PackQuery(Dictionary<int, T> components, int tick)
                {
                    _components = components;
                    _tick = tick;
                }

                public void Update(ref History<T> history, ref History<SyncId> id)
                {
                    var historyComponent = history[_tick];
                    var idComponent = id[_tick];
                    if (historyComponent.HasValue && idComponent.HasValue)
                        _components[idComponent.Value] = historyComponent.Value;
                }
            }
        }

        private readonly struct ApplyAction : IStructGenericAction
        {
            private readonly IEntityRegistry _entityRegistryService;
            private readonly WorldData _worldData;
            private readonly World _world;

            public ApplyAction(IEntityRegistry entityRegistryService, WorldData worldData, World world)
            {
                _entityRegistryService = entityRegistryService;
                _worldData = worldData;
                _world = world;
            }

            public void Invoke<T>() where T : struct
            {
                foreach (var (id, data) in _worldData.Get<T>())
                {
                    _entityRegistryService.TryGet(new() { Value = id }, out var entity);
                    _world.AddOrGet<T>(entity) = data;
                }
            }
        }
    }

}