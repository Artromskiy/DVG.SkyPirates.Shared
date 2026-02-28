namespace DVG.SkyPirates.Shared.IServices.TickableExecutors
{
    public interface ITickableService<T> where T : ITickableExecutor
    {
        void Tick(int tick);
    }
}
