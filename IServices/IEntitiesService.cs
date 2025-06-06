using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.IServices
{
    public interface IEntitiesService
    {
        void AddEntity(int entityId, object entity);
        void RemoveEntity(int entityId);
        bool RemoveEntity<T>(int entityId, out T entity) where T : class;
        bool TryGetEntity<T>(int entityId, out T entity) where T : class;
        IEnumerable<T> GetEntities<T>() where T : class;
        bool HasEntity(int entityId);
        bool HasEntity<T>(int entityId);
        void RemoveAllExcept(HashSet<int> entityIds);
    }
}
