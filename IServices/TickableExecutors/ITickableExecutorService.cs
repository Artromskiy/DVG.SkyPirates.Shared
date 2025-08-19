namespace DVG.SkyPirates.Shared.IServices.TickableExecutors
{
    public interface ITickableExecutorService
    {
        void Tick(int tick, fix deltaTime);
    }
}
