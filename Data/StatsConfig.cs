using DVG.SkyPirates.Shared.Ids;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Data
{
    public sealed class UnitsStatsConfig : Dictionary<UnitId, ComponentsData> { }
    public sealed class RocksStatsConfig : Dictionary<RockId, ComponentsData> { }
    public sealed class TreesStatsConfig : Dictionary<TreeId, ComponentsData> { }
    public sealed class GoodsStatsConfig : Dictionary<GoodsId, ComponentsData> { }
    public sealed class CactusesStatsConfig : Dictionary<CactusId, ComponentsData> { }
}
