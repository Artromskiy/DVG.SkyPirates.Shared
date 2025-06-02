using DVG.SkyPirates.Shared.IServices;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Services
{
    public class EntitiesService : IEntitiesService
    {
        private readonly Dictionary<int, object> _instances = new Dictionary<int, object>();
        public void AddEntity(int instanceId, object instance)
        {
            _instances.Add(instanceId, instance);
        }

        public bool HasEntity(int instanceId)
        {
            throw new System.NotImplementedException();
        }

        public bool HasEntity<T>(int instanceId)
        {
            throw new System.NotImplementedException();
        }

        public void RemoveEntity(int instanceId)
        {
            _instances.Remove(instanceId);
        }

        public bool RemoveEntity<T>(int instanceId, out T instance) where T : class
        {
            if (TryGetEntity(instanceId, out instance))
            {
                RemoveEntity(instanceId);
                return true;
            }
            return false;
        }

        public bool TryGetEntity<T>(int instanceId, out T instance) where T : class
        {
            instance = default!;
            if (!_instances.TryGetValue(instanceId, out var instanceObj))
                return false;
            instance = (instanceObj as T)!;
            return instance == null;
        }
    }
}
