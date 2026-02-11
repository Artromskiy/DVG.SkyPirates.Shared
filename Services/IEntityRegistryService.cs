using Arch.Core;
using DVG.Components;
using DVG.Core.Collections;
using DVG.SkyPirates.Shared.IServices;

namespace DVG.SkyPirates.Shared.Services
{
    public class EntityRegistryService : IEntityRegistryService
    {
        private readonly Lookup<Entity> _idToEntity = new();
        private int _entityIdCounter;
        public void Register(Entity entity, SyncId syncId)
        {
            _idToEntity[syncId.Value] = entity;
            _entityIdCounter = Maths.Max(_entityIdCounter, syncId.Value);
        }

        public SyncId Reserve()
        {
            return new() { Value = ++_entityIdCounter };
        }

        public SyncIdReserve Reserve(int count)
        {
            int current = _entityIdCounter + 1;
            int last = _entityIdCounter += count;
            return new() { Current = current, Last = last };
        }

        public bool TryGet(SyncId syncId, out Entity entity)
        {
            return _idToEntity.TryGetValue(syncId.Value, out entity);
        }
    }
}
