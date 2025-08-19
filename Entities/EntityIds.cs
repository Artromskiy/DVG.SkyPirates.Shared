using Arch.Core;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Entities
{
    public static class EntityIds
    {
        private static readonly Dictionary<int, Entity> _entityIds = new Dictionary<int, Entity>();

        public static Entity Get(int id)
        {
            if (!_entityIds.TryGetValue(id, out var entity))
            {
                _entityIds[id] = entity = World.Worlds[0].Create();
            }
            return entity;
        }
    }
}
