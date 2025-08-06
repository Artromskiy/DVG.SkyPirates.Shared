using DVG.SkyPirates.Shared.IServices;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DVG.SkyPirates.Shared.Services
{
    public class EntitiesService : IEntitiesService
    {
        public int CurrentTick { get; set; }

        private readonly List<Dictionary<int, object>> _entities = new List<Dictionary<int, object>>();
        private Dictionary<int, object> CurrentEntities => CurrentTick < _entities.Count ? _entities[CurrentTick] : new Dictionary<int, object>();

        public void AddEntity(int entityId, object instance) => CurrentEntities.Add(entityId, instance);
        public bool HasEntity(int entityId) => CurrentEntities.ContainsKey(entityId);
        public void RemoveEntity(int entityId) => CurrentEntities.Remove(entityId);
        public T GetEntity<T>(int entityId)
            where T : class
        {
            CurrentEntities.TryGetValue(entityId, out var entityObj);
            if (!(entityObj is T castedEntity))
                throw new KeyNotFoundException($"Entity with id {entityId} not found");
            return castedEntity;
        }

        public bool TryGetEntity<T>(int entityId, [NotNullWhen(true)] out T? entity)
            where T : class
        {
            entity = null;
            if (!CurrentEntities.TryGetValue(entityId, out var entityObj))
                return false;
            entity = entityObj as T;
            return !(entity is null);
        }

        public IReadOnlyCollection<int> GetEntityIds() => CurrentEntities.Keys;

        public void CopyPreviousEntities()
        {
            CurrentEntities.Clear();
            int prevFrame = CurrentTick - 1;
            if (prevFrame >= _entities.Count || prevFrame < 0)
                return;

            foreach (var (id, obj) in _entities[prevFrame])
                CurrentEntities.Add(id, obj);
        }
    }
}
