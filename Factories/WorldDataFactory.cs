using Arch.Core;
using DVG.Core;
using DVG.Core.Components;
using DVG.SkyPirates.Shared.IFactories;
using DVG.SkyPirates.Shared.Tools;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Data
{
    public class WorldDataFactory : IWorldDataFactory
    {
        private class Description<T>
        {
            public QueryDescription Desc = new QueryDescription().WithAll<T>();
        }
        private readonly GenericCollection _desc = new();

        private readonly ICommandEntityFactory _commandEntityFactory;
        private readonly World _world;

        public WorldDataFactory(World world, ICommandEntityFactory commandEntityFactory)
        {
            _world = world;
            _commandEntityFactory = commandEntityFactory;
        }

        public WorldData Create()
        {
            var worldData = new WorldData();
            var packAction = new PackAction(_world, worldData, _desc, _commandEntityFactory);
            ComponentIds.ForEachData(ref packAction);
            return worldData;
        }

        public void Extract(WorldData worldData)
        {
            var unpackAction = new ExtractAction(_world, worldData, _commandEntityFactory);
            ComponentIds.ForEachData(ref unpackAction);
        }

        private readonly struct PackAction : IStructGenericAction
        {
            private readonly ICommandEntityFactory _commandEntityFactory;
            private readonly GenericCollection _desc;
            private readonly WorldData _worldData;
            private readonly World _world;

            public PackAction(World world, WorldData entities, GenericCollection desc, ICommandEntityFactory commandEntityFactory)
            {
                _world = world;
                _worldData = entities;
                _desc = desc;
                _commandEntityFactory = commandEntityFactory;
            }

            public readonly void Invoke<T>() where T : struct
            {
                var components = _worldData.Get<T>();
                var query = new PackQuery<T>(components, _commandEntityFactory);
                var desc = _desc.Get<Description<T>>().Desc;
                _world.InlineEntityQuery<PackQuery<T>, T>(desc, ref query);
            }

            private readonly struct PackQuery<T> : IForEachWithEntity<T> where T : struct
            {
                private readonly ICommandEntityFactory _commandEntityFactory;
                private readonly Dictionary<int, T> _components;

                public PackQuery(Dictionary<int, T> components, ICommandEntityFactory commandEntityFactory)
                {
                    _components = components;
                    _commandEntityFactory = commandEntityFactory;
                }

                public void Update(Entity entity, ref T component)
                {
                    var id = _commandEntityFactory.Get(entity);
                    _components[id] = component;
                }
            }
        }

        private readonly struct ExtractAction : IStructGenericAction
        {
            private readonly ICommandEntityFactory _commandEntityFactory;
            private readonly WorldData _worldData;
            private readonly World _world;

            public ExtractAction(World world, WorldData entities, ICommandEntityFactory commandEntityFactory)
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