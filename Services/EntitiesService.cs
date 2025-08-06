using DVG.SkyPirates.Shared.IServices;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DVG.SkyPirates.Shared.Services
{
    public class EntitiesService : IEntitiesService
    {
        public int CurrentTick { get; set; }

        private readonly Dictionary<int, Dictionary<int, object>> _timeline = new Dictionary<int, Dictionary<int, object>>();
        private Dictionary<int, object> CurrentEntities => GetEntities(CurrentTick);
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

        public Dictionary<int, object> GetEntities(int tick)
        {
            if (!_timeline.TryGetValue(tick, out var entities))
                _timeline[tick] = entities = new Dictionary<int, object>();
            return entities;
        }
    }
}
