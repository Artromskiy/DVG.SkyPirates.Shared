using Arch.Core;
using DVG.Core;
using DVG.Core.Components;
using DVG.SkyPirates.Shared.Components.Special;
using DVG.SkyPirates.Shared.IFactories;
using DVG.SkyPirates.Shared.Tools;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Data
{
    // TODO Add dead components, as they can rollback?
    public class WorldDataFactory : IWorldDataFactory
    {
        private class Description<T>
        {
            public QueryDescription Desc = new QueryDescription().WithAll<T, SyncId>();
        }
        private readonly GenericCollection _desc = new();

        private readonly IEntityFactory _commandEntityFactory;
        private readonly World _world;

        public WorldDataFactory(World world, IEntityFactory commandEntityFactory)
        {
            _world = world;
            _commandEntityFactory = commandEntityFactory;
        }

        public WorldData Create()
        {
            var worldData = new WorldData();
            var packAction = new PackAction(_world, worldData, _desc);
            HistoryIds.ForEachData(ref packAction);
            return worldData;
        }

        public void Extract(WorldData worldData)
        {
            var unpackAction = new ExtractAction(_world, worldData, _commandEntityFactory);
            HistoryIds.ForEachData(ref unpackAction);
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
            private readonly IEntityFactory _commandEntityFactory;
            private readonly WorldData _worldData;
            private readonly World _world;

            public ExtractAction(World world, WorldData entities, IEntityFactory commandEntityFactory)
            {
                _world = world;
                _worldData = entities;
                _commandEntityFactory = commandEntityFactory;
            }

            public void Invoke<T>() where T : struct
            {
                foreach (var (id, data) in _worldData.Get<T>())
                {
                    var entity = _commandEntityFactory.Create(id);
                    _world.AddOrGet<T>(entity) = data;
                }
            }
        }
    }

}