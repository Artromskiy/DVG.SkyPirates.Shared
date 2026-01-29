using Arch.Core;
using DVG.Core;
using DVG.Core.Components;
using DVG.SkyPirates.Shared.Components.Special;
using DVG.SkyPirates.Shared.Entities;
using DVG.SkyPirates.Shared.Tools;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Data
{
    public static class WorldDataFactory
    {
        private class Description<T>
        {
            public QueryDescription Desc = new QueryDescription().WithAll<T>();
        }
        private static readonly GenericCollection _desc = new();

        public static WorldData Create(World world)
        {
            var worldData = new WorldData();
            var packAction = new PackAction(_desc, worldData, world);
            ComponentIds.ForEachData(ref packAction);
            return worldData;
        }

        public static void Extract(World world, WorldData worldData)
        {
            var unpackAction = new UnpackAction(worldData, world);
            ComponentIds.ForEachData(ref unpackAction);
        }

        private readonly struct PackAction : IStructGenericAction
        {
            private readonly GenericCollection _desc;
            private readonly WorldData _worldData;
            private readonly World _world;

            public PackAction(GenericCollection desc, WorldData entities, World world)
            {
                _desc = desc;
                _worldData = entities;
                _world = world;
            }

            public readonly void Invoke<T>() where T : struct
            {
                var components = _worldData.Get<T>();
                var query = new PackQuery<T>(components);
                var desc = _desc.Get<Description<T>>().Desc;
                _world.InlineEntityQuery<PackQuery<T>, T>(desc, ref query);
            }

            private readonly struct PackQuery<T> : IForEachWithEntity<T> where T : struct
            {
                private readonly Dictionary<int, T> _components;

                public PackQuery(Dictionary<int, T> components)
                {
                    _components = components;
                }

                public void Update(Entity entity, ref T component)
                {
                    _components[entity.Id] = component;
                }
            }
        }

        private readonly struct UnpackAction : IStructGenericAction
        {
            private readonly WorldData _worldData;
            private readonly World _world;

            public UnpackAction(WorldData entities, World world)
            {
                _worldData = entities;
                _world = world;
            }

            public void Invoke<T>() where T : struct
            {
                foreach (var (id, data) in _worldData.Get<T>())
                {
                    var entity = EntityIds.Get(id);

                    if (_world.Has<Free>(entity))
                        _world.Remove<Free>(entity);

                    ref var cmp = ref _world.AddOrGet<T>(entity);
                    cmp = data;
                }
            }
        }
    }

}