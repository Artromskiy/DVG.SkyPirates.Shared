using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DVG.SkyPirates.Shared.Data
{
    [Serializable]
    [DataContract]
    public class WorldData
    {
        [DataMember(Order = 0)]
        public readonly Dictionary<string, List<(int entity, string data)>> Entities;

        public WorldData(Dictionary<string, List<(int entity, string data)>> entities)
        {
            Entities = entities;
        }
    }
}
