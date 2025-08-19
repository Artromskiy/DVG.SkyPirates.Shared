namespace DVG.SkyPirates.Shared.IServices.TickableExecutors
{
    public interface IPostTickableExecutorService
    {
        void Tick(int tick, fix deltaTime);
    }
}
