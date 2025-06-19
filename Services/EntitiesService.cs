using DVG.SkyPirates.Shared.IServices;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DVG.SkyPirates.Shared.Services
{
    public class EntitiesService : IEntitiesService
    {
        private readonly Dictionary<int, object> _entities = new Dictionary<int, object>();
        private readonly List<int> _keys = new List<int>();
        private int _newEntityId;

        public void AddEntity(int entityId, object instance) => _entities.Add(entityId, instance);
        public bool HasEntity(int entityId) => _entities.ContainsKey(entityId);
        public void RemoveEntity(int entityId) => _entities.Remove(entityId);
        public T? GetEntity<T>(int entityId)
            where T : class
        {
            _entities.TryGetValue(entityId, out var entityObj);
            return entityObj as T;
        }

        public bool TryGetEntity<T>(int entityId, [NotNullWhen(true)] out T? entity)
            where T : class
        {
            entity = null;
            if (!_entities.TryGetValue(entityId, out var entityObj))
                return false;
            entity = entityObj as T;
            return !(entity is null);
        }

        public IReadOnlyCollection<int> GetEntityIds() => _entities.Keys;

        public void RemoveAllExcept(HashSet<int> entityIds)
        {
            _keys.Clear();
            _keys.AddRange(_entities.Keys);

            foreach (var item in _keys)
                if (!entityIds.Contains(item))
                    _entities.Remove(item);
        }

        public int NewEntityId() => ++_newEntityId;

    }
}
