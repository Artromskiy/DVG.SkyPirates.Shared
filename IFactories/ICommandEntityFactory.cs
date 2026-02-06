using Arch.Core;
using DVG.Core;

namespace DVG.SkyPirates.Shared.IFactories
{
    public interface ICommandEntityFactory : IFactory<Entity, int>
    {
        Entity Get(int entityId);
        int Get(Entity entity);
        int Next();
    }
}
