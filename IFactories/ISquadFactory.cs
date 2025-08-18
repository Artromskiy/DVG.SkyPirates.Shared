using DVG.Core;
using DVG.SkyPirates.Shared.Commands;
using DVG.SkyPirates.Shared.Entities;

namespace DVG.SkyPirates.Shared.IFactories
{
    public interface ISquadFactory : IFactory<SquadEntity, Command<SpawnSquadCommand>> { }
}
