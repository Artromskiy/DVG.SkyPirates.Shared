using DVG.SkyPirates.Shared.IServices;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Services
{
    public class EntitiesService : IEntitiesService
    {
        private readonly Dictionary<int, object> _entities = new Dictionary<int, object>();
        private readonly List<int> _keys = new List<int>();
        public void AddEntity(int entityId, object instance)
        {
            _entities.Add(entityId, instance);
        }

        public bool HasEntity(int entityId)
        {
            throw new System.NotImplementedException();
        }

        public bool HasEntity<T>(int entityId)
        {
            throw new System.NotImplementedException();
        }

        public void RemoveEntity(int entityId)
        {
            _entities.Remove(entityId);
        }

        public bool RemoveEntity<T>(int entityId, out T entity) where T : class
        {
            if (TryGetEntity(entityId, out entity))
            {
                RemoveEntity(entityId);
                return true;
            }
            return false;
        }

        public bool TryGetEntity<T>(int entityId, out T entity) where T : class
        {
            entity = default!;
            if (!_entities.TryGetValue(entityId, out var entityObj))
                return false;
            entity = (entityObj as T)!;
            return entity == null;
        }

        public IEnumerable<T> GetEntities<T>() where T : class
        {
            foreach (var item in _entities)
                if (item is T genericItem)
                    yield return genericItem;
        }

        public void RemoveAllExcept(HashSet<int> entityIds)
        {
            _keys.Clear();
            _keys.AddRange(_entities.Keys);

            foreach (var item in _keys)
                if (!entityIds.Contains(item))
                    _entities.Remove(item);

        }
    }
}
