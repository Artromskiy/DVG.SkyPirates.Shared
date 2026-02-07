using DVG.Core.Components.Attributes;
using DVG.SkyPirates.Shared.Ids;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DVG.SkyPirates.Shared.Components.Config
{
    [Component]
    public struct HexMap
    {
        [DataMember(Order = 0)]
        public Dictionary<int3, TileId> Data;
    }
}