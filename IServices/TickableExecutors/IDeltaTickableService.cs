namespace DVG.SkyPirates.Shared.IServices.TickableExecutors
{
    public interface IDeltaTickableService<T> where T : IDeltaTickableExecutor
    {
        void Tick(int tick, fix deltaTime);
    }
}
