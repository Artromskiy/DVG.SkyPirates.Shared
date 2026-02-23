using DVG.Components.Attributes;
using DVG.SkyPirates.Shared.Ids;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Components.Config
{
    [Component(true)]
    public struct HexMap
    {
        public Dictionary<int3, TileId> Data;
    }
}