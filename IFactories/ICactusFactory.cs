using Arch.Core;
using DVG.Core;
using DVG.SkyPirates.Shared.Ids;

namespace DVG.SkyPirates.Shared.IFactories
{
    public interface ICactusFactory : IFactory<Entity, (CactusId CactusId, int EntityId)> { }
}