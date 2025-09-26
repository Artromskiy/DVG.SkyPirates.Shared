using Arch.Core;
using DVG.Core;
using DVG.Core.Components;
using DVG.SkyPirates.Shared.Entities;
using DVG.SkyPirates.Shared.Tools.Json;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Data
{
    public class WorldSerialization
    {
        public Dictionary<string, List<(int entity, string data)>> _entities;

        public WorldSerialization(World world)
        {
            _entities = new Dictionary<string, List<(int entity, string data)>>();
            var serializationAction = new SerializationAction(_entities, world);
            ComponentIds.ForEachData(ref serializationAction);
        }

        public void Apply(World world)
        {
            var deserializationAction = new DeserializationAction(_entities, world);
            ComponentIds.ForEachData(ref deserializationAction);
        }

        private readonly struct SerializationAction : IStructGenericAction
        {
            private readonly Dictionary<string, List<(int entity, string data)>> _entities;
            private readonly World _world;

            public SerializationAction(Dictionary<string, List<(int entity, string data)>> entities, World world)
            {
                _entities = entities;
                _world = world;
            }

            public readonly void Invoke<T>() where T : struct
            {
                var components = new List<(int entity, string data)>();
                _entities.Add(typeof(T).Name, components);
                var query = new SerializationQuery<T>(components);
                _world.InlineEntityQuery<SerializationQuery<T>, T>(new(), ref query);
            }

            private readonly struct SerializationQuery<T> : IForEachWithEntity<T> where T : struct
            {
                private readonly List<(int entity, string data)> _components;

                public SerializationQuery(List<(int entity, string data)> components)
                {
                    _components = components;
                }

                public void Update(Entity entity, ref T component)
                {
                    _components.Add((entity.Id, Serialization.Serialize(component)));
                }
            }
        }

        private readonly struct DeserializationAction : IStructGenericAction
        {
            private readonly Dictionary<string, List<(int entity, string data)>> _entities;
            private readonly World _world;

            public DeserializationAction(Dictionary<string, List<(int entity, string data)>> entities, World world)
            {
                _entities = entities;
                _world = world;
            }

            public void Invoke<T>() where T : struct
            {
                if (!_entities.TryGetValue(typeof(T).Name, out var components))
                    return;

                foreach (var (entity, data) in components)
                {
                    ref var cmp = ref _world.AddOrGet<T>(EntityIds.Get(entity));
                    cmp = Serialization.Deserialize<T>(data);
                }
            }
        }
    }
}
