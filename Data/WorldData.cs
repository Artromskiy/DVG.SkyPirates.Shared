using DVG.SkyPirates.Shared.Tools.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DVG.SkyPirates.Shared.Data
{
    [Serializable]
    [DataContract]
    public class WorldData
    {
        [DataMember(Order = 0)]
        public readonly Dictionary<string, IList> Entities;

        public WorldData(Dictionary<string, IList> entities)
        {
            Entities = entities;
        }

        public static WorldData Create() => new(new());

        public void Set<T>(List<(int entity, T data)> entities)
            where T : struct
        {
            Entities[typeof(T).Name] = entities.ConvertAll(e => (e.entity, SerializationUTF8.Serialize(e.data)));
        }
    }
}
