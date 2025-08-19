using Arch.Core;
using DVG.Core;
using DVG.SkyPirates.Shared.Commands;

namespace DVG.SkyPirates.Shared.IFactories
{
    public interface IUnitFactory : IFactory<Entity, Command<SpawnUnitCommand>> { }
}
