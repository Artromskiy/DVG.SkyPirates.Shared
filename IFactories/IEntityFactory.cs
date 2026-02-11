using Arch.Core;
using DVG.Core;

namespace DVG.SkyPirates.Shared.IFactories
{
    public interface IEntityFactory : IFactory<Entity, int>
    {
        Entity Get(int entityId);
        int Next();
    }
    //public interface IEntityFactory : IFactory<Entity, (int SyncId, )>
}
