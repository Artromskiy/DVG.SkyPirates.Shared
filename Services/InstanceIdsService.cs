using DVG.SkyPirates.Shared.IServices;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Services
{
    public class InstanceIdsService : IInstanceIdsService
    {
        private readonly Dictionary<int, object> _instances = new Dictionary<int, object>();
        public void AddInstance(int instanceId, object instance)
        {
            _instances.Add(instanceId, instance);
        }

        public void RemoveInstance(int instanceId)
        {
            _instances.Remove(instanceId);
        }

        public bool RemoveInstance<T>(int instanceId, out T instance) where T : class
        {
            if (TryGetInstance(instanceId, out instance))
            {
                RemoveInstance(instanceId);
                return true;
            }
            return false;
        }

        public bool TryGetInstance<T>(int instanceId, out T instance) where T : class
        {
            instance = default!;
            if (!_instances.TryGetValue(instanceId, out var instanceObj))
                return false;
            instance = (instanceObj as T)!;
            return instance == null;
        }
    }
}
