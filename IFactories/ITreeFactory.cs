using Arch.Core;
using DVG.Core;
using DVG.SkyPirates.Shared.Ids;

namespace DVG.SkyPirates.Shared.IFactories
{
    public interface ITreeFactory : IFactory<Entity, (TreeId TreeId, int EntityId)> { }
}