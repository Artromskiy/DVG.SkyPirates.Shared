using DVG.Components.Attributes;
using DVG.SkyPirates.Shared.Ids;
using System.Collections.Frozen;

namespace DVG.SkyPirates.Shared.Components.Config
{
    [Component(true)]
    public struct HexMap
    {
        public FrozenDictionary<int3, TileId> Data;
    }
}