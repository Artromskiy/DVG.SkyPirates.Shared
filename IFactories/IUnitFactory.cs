using Arch.Core;
using DVG.Core;
using DVG.SkyPirates.Shared.Ids;

namespace DVG.SkyPirates.Shared.IFactories
{
    public interface IUnitFactory : IFactory<Entity, (fix3 Position, int TeamId, int Level, UnitId UnitId, int EntityId)> { }
}
