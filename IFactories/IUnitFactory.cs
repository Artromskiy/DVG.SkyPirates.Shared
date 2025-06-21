using DVG.Core;
using DVG.SkyPirates.Shared.Commands;
using DVG.SkyPirates.Shared.Presenters;

namespace DVG.SkyPirates.Shared.IFactories
{
    public interface IUnitFactory : IFactory<UnitPm, Command<SpawnUnit>> { }
}
