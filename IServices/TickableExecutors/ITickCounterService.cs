namespace DVG.SkyPirates.Shared.IServices.TickableExecutors
{
    public interface ITickCounterService : ITickableExecutor
    {
        int TickCounter { get; }
    }
}
