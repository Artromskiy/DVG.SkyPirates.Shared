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
        public readonly Dictionary<string, IDictionary> Entities;

        public WorldData(Dictionary<string, IDictionary> entities)
        {
            Entities = entities;
        }

        public static WorldData Create() => new(new());

        public void Set<T>(Dictionary<int, T> entities)
            where T : struct
        {
            Entities[typeof(T).Name] = entities;
        }
    }
}
