using Arch.Core;
using DVG.Core;
using DVG.SkyPirates.Shared.Ids;

namespace DVG.SkyPirates.Shared.IFactories
{
    public interface IUnitFactory : IFactory<Entity, (UnitId UnitId, int EntityId)> { }
}
