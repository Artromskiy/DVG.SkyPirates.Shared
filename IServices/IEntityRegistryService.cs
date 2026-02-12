using Arch.Core;
using DVG.Components;

namespace DVG.SkyPirates.Shared.IServices
{
    public interface IEntityRegistryService
    {
        void Register(SyncId syncId);
        void Register(Entity entity, SyncId syncId);
        void Register(SyncIdReserve syncIdReserve);
        SyncId Reserve();
        SyncIdReserve Reserve(int count);
        bool TryGet(SyncId syncId, out Entity entity);
    }
}
