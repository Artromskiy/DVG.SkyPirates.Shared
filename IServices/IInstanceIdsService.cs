namespace DVG.SkyPirates.Shared.IServices
{
    public interface IInstanceIdsService
    {
        void AddInstance(int instanceId, object instance);
        void RemoveInstance(int instanceId);
        bool RemoveInstance<T>(int instanceId, out T instance) where T : class;
        bool TryGetInstance<T>(int instanceId, out T instance) where T : class;
        bool HasInstance(int instanceId);
        bool HasInstance<T>(int instanceId);
    }
}
