namespace DVG.SkyPirates.Shared.IServices.TickableExecutors
{
    public interface IPreTickableExecutor
    {
        void Tick(int tick, fix deltaTime);
    }
}
