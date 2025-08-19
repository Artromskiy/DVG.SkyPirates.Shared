namespace DVG.SkyPirates.Shared.IServices.TickableExecutors
{
    public interface IPreTickableExecutorService
    {
        void Tick(int tick, fix deltaTime);
    }
}
