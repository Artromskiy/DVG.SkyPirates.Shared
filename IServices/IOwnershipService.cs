namespace DVG.SkyPirates.Shared.IServices
{
    public interface IOwnershipService
    {
        bool HasOwnership(int clientId, int entityId);
        void SetOwner(int clientId, int entityId);
        int[] GetOwnedEntities(int clientId);
    }
}
