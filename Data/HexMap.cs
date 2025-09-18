using DVG.SkyPirates.Shared.Ids;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DVG.SkyPirates.Shared.Data
{
    [Serializable]
    [DataContract]
    public class HexMap
    {
        [DataMember(Order = 0)]
        public Dictionary<int3, TileId> Map { get; set; }
    }
}
