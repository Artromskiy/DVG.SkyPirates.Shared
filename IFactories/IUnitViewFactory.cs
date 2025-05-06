using DVG.Core;
using DVG.SkyPirates.Shared.Ids;
using DVG.SkyPirates.Shared.IViews;

namespace DVG.SkyPirates.Shared.IFactories
{
    public interface IUnitViewFactory : IFactory<IUnitView, (UnitId unitId, int level, int merge)> { }
}
