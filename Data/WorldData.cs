using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DVG.SkyPirates.Shared.Data
{
    public class WorldData
    {
        [DataMember]
        private readonly Dictionary<string, List<(int entity, string data)>> _entities;
        [IgnoreDataMember]
        public IReadOnlyDictionary<string, List<(int entity, string data)>> Entities => _entities;

        public WorldData(Dictionary<string, List<(int entity, string data)>> entities)
        {
            _entities = entities;
        }
    }
}
