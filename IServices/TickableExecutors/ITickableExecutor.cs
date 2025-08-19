namespace DVG.SkyPirates.Shared.IServices.TickableExecutors
{
    public interface ITickableExecutor
    {
        void Tick(int tick, fix deltaTime);
    }
}
