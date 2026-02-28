namespace DVG.SkyPirates.Shared.IServices.TickableExecutors
{
    public interface IDeltaTickableService<T>
    {
        void Tick(int tick, fix deltaTime);
    }
}
