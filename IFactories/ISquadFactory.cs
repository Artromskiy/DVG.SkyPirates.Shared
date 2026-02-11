using Arch.Core;
using DVG.Commands;
using DVG.Core;
using DVG.SkyPirates.Shared.Commands;

namespace DVG.SkyPirates.Shared.IFactories
{
    public interface ISquadFactory : IFactory<Entity, Command<SpawnSquadCommand>> { }
}
