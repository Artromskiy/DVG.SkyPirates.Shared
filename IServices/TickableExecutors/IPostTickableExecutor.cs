namespace DVG.SkyPirates.Shared.IServices.TickableExecutors
{
    public interface IPostTickableExecutor
    {
        void Tick(int tick, fix deltaTime);
    }
}
