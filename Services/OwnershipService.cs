using DVG.SkyPirates.Shared.IServices;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;

namespace DVG.SkyPirates.Shared.Services
{
    public class OwnershipService : IOwnershipService
    {
        private readonly Dictionary<int, int> _entityToClient = new Dictionary<int, int>();

        public bool HasOwnership(int clientId, int entityId)
        {
            return _entityToClient.TryGetValue(entityId, out var owner) && owner == clientId;
        }

        public void SetOwner(int clientId, int entityId)
        {
            _entityToClient[entityId] = clientId;
        }

        public int[] GetOwnedEntities(int clientId)
        {
            return _entityToClient.Where(kv => kv.Value == clientId).Select(kv => kv.Key).ToArray();
        }
    }
}
