using DVG.SkyPirates.Server.IServices;

namespace DVG.SkyPirates.Shared.Services
{
    public class OwnershipService : IOwnershipService
    {

        public bool HasOwnership(int clientId, int entityId)
        {
            return true;
        }

        public bool SetOwner(int ownerClientId, int entityId)
        {
            return true;
        }
    }
}
