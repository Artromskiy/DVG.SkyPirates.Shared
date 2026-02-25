using DVG.SkyPirates.Shared.Ids;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Data
{
    public sealed class UnitsStatsConfig : Dictionary<UnitId, ComponentsSet> { }
    public sealed class RocksStatsConfig : Dictionary<RockId, ComponentsSet> { }
    public sealed class TreesStatsConfig : Dictionary<TreeId, ComponentsSet> { }
    public sealed class GoodsStatsConfig : Dictionary<GoodsId, ComponentsSet> { }
    public sealed class CactusesStatsConfig : Dictionary<CactusId, ComponentsSet> { }
}
