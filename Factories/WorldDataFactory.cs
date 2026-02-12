using Arch.Core;
using DVG.Components;
using DVG.SkyPirates.Shared.Data;
using DVG.SkyPirates.Shared.IFactories;
using DVG.SkyPirates.Shared.IServices;
using DVG.SkyPirates.Shared.Tools;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Factories
{
    public class WorldDataFactory : IWorldDataFactory
    {
        private class Description<T>
        {
            public QueryDescription Desc = new QueryDescription().WithAll<T, SyncId>();
        }
        private readonly GenericCollection _desc = new();

        private readonly IEntityRegistryService _entityRegistryService;
        private readonly IEntityFactory _entityFactory;
        private readonly World _world;

        public WorldDataFactory(IEntityRegistryService entityRegistryService, IEntityFactory entityFactory, World world)
        {
            _entityRegistryService = entityRegistryService;
            _entityFactory = entityFactory;
            _world = world;
        }

        public WorldData Create()
        {
            var worldData = new WorldData();
            var packAction = new PackAction(_world, worldData, _desc);
            HistoryComponentsRegistry.ForEachData(ref packAction);
            return worldData;
        }

        public void Extract(WorldData worldData)
        {
            foreach (var syncId in worldData.Get<SyncId>().Values)
                _entityRegistryService.Reserve(syncId);

            foreach (var syncIdReserve in worldData.Get<SyncIdReserve>().Values)
                _entityRegistryService.Reserve(syncIdReserve);

            foreach (var syncId in worldData.Get<SyncId>().Values)
                _entityFactory.Create(new(syncId, default, default));

            var unpackAction = new ExtractAction(_entityRegistryService, worldData, _world);
            HistoryComponentsRegistry.ForEachData(ref unpackAction);
        }

        private readonly struct PackAction : IStructGenericAction
        {
            private readonly GenericCollection _desc;
            private readonly WorldData _worldData;
            private readonly World _world;

            public PackAction(World world, WorldData entities, GenericCollection desc)
            {
                _world = world;
                _worldData = entities;
                _desc = desc;
            }

            public readonly void Invoke<T>() where T : struct
            {
                var components = _worldData.Get<T>();
                var query = new PackQuery<T>(components);
                var desc = _desc.Get<Description<T>>().Desc;
                _world.InlineQuery<PackQuery<T>, T, SyncId>(desc, ref query);
            }

            private readonly struct PackQuery<T> : IForEach<T, SyncId> where T : struct
            {
                private readonly Dictionary<int, T> _components;

                public PackQuery(Dictionary<int, T> components)
                {
                    _components = components;
                }

                public void Update(ref T component, ref SyncId id)
                {
                    _components[id.Value] = component;
                }
            }
        }

        private readonly struct ExtractAction : IStructGenericAction
        {
            private readonly IEntityRegistryService _entityRegistryService;
            private readonly WorldData _worldData;
            private readonly World _world;

            public ExtractAction(IEntityRegistryService entityRegistryService, WorldData worldData, World world)
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