using DVG.Core;
using DVG.SkyPirates.Shared.Ids;
using DVG.SkyPirates.Shared.Models;

namespace DVG.SkyPirates.Shared.IFactories
{
    public interface IUnitModelFactory : IFactory<UnitModel, (UnitId unitId, int level, int merge)> { }
}
