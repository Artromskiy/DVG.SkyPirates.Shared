using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DVG.SkyPirates.Shared.IServices
{
    public interface IEntitiesService
    {
        void AddEntity(int entityId, object entity);
        void RemoveEntity(int entityId);
        bool HasEntity(int entityId);
        T? GetEntity<T>(int entityId) where T : class;
        bool TryGetEntity<T>(int entityId, out T? entity) where T : class;
        IEnumerable<T> GetEntities<T>() where T : class;
        void RemoveAllExcept(HashSet<int> entityIds);

        int NewEntityId();
    }
}
