using Arch.Core;
using DVG.Core;
using DVG.Core.Components;
using DVG.SkyPirates.Shared.Entities;
using DVG.SkyPirates.Shared.Tools;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Data
{
    public static class WorldDataSerializer
    {
        private class Description<T>
        {
            public QueryDescription Desc = new QueryDescription().WithAll<T>();
        }
        private static readonly GenericCollection _desc = new();

        public static WorldData Serialize(World world)
        {
            var worldData = new WorldData();
            var serializationAction = new SerializationAction(_desc, worldData, world);
            ComponentIds.ForEachData(ref serializationAction);
            return worldData;
        }

        public static void Deserialize(World world, WorldData worldData)
        {
            var deserializationAction = new DeserializationAction(worldData, world);
            ComponentIds.ForEachData(ref deserializationAction);
        }

        private readonly struct SerializationAction : IStructGenericAction
        {
            private readonly GenericCollection _desc;
            private readonly WorldData _worldData;
            private readonly World _world;

            public SerializationAction(GenericCollection desc, WorldData entities, World world)
            {
                _desc = desc;
                _worldData = entities;
                _world = world;
            }

            public readonly void Invoke<T>() where T : struct
            {
                var components = _worldData.Get<T>();
                var query = new SerializationQuery<T>(components);
                var desc = _desc.Get<Description<T>>().Desc;
                _world.InlineEntityQuery<SerializationQuery<T>, T>(desc, ref query);
            }

            private readonly struct SerializationQuery<T> : IForEachWithEntity<T> where T : struct
            {
                private readonly Dictionary<int, T> _components;

                public SerializationQuery(Dictionary<int, T> components)
                {
                    _components = components;
                }

                public void Update(Entity entity, ref T component)
                {
                    _components[entity.Id] = component;
                }
            }
        }

        private readonly struct DeserializationAction : IStructGenericAction
        {
            private readonly WorldData _worldData;
            private readonly World _world;

            public DeserializationAction(WorldData entities, World world)
            {
                _worldData = entities;
                _world = world;
            }

            public void Invoke<T>() where T : struct
            {
                foreach (var (entity, data) in _worldData.Get<T>())
                {
                    ref var cmp = ref _world.AddOrGet<T>(EntityIds.Get(entity));
                    cmp = data;
                }
            }
        }
    }

}