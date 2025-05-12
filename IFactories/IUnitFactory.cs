using DVG.Core;
using DVG.SkyPirates.Shared.Ids;
using DVG.SkyPirates.Shared.Presenters;

namespace DVG.SkyPirates.Shared.IFactories
{
    public interface IUnitFactory : IFactory<UnitPm, (UnitId unitId, int level, int merge)> { }
}
