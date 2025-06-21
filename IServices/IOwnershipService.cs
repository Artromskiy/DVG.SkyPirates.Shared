using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.IServices
{
    public interface IOwnershipService
    {
        bool HasOwnership(int clientId, int entityId);
        void SetOwner(int clientId, int entityId);
        void RemoveOwner(int entityId);
        int[] GetOwnedEntities(int clientId);

        void RemoveAllExcept(HashSet<int> entityIds);
    }
}
