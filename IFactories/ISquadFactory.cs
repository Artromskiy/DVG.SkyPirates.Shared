using Arch.Core;
using DVG.Core;
using DVG.SkyPirates.Shared.Components.Runtime;
using DVG.SkyPirates.Shared.Data;

namespace DVG.SkyPirates.Shared.IFactories
{
    public interface ISquadFactory : IFactory<Entity, (EntityParameters entityParameters, Team team)> { }
}
