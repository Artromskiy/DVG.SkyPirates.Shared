namespace DVG.SkyPirates.Shared.IServices.TickableExecutors
{
    public interface ITickableService<T>
    {
        void Tick(int tick);
    }
}
