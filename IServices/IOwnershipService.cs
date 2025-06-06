namespace DVG.SkyPirates.Server.IServices
{
    public interface IOwnershipService
    {
        bool HasOwnership(int clientId, int entityId);
        bool SetOwner(int clientId, int entityId);
    }
}
