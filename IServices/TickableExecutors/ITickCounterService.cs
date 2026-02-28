using DVG.SkyPirates.Shared.IServices.TickableExecutors;

namespace DVG.SkyPirates.Client.IServices
{
    public interface ITickCounterService : ITickableExecutor
    {
        int TickCounter { get; }
    }
}
