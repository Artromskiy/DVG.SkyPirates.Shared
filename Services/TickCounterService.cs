using DVG.SkyPirates.Shared.IServices.TickableExecutors;

namespace DVG.SkyPirates.Shared.Services
{
    public class TickCounterService : ITickCounterService, ITickableExecutor
    {
        public int TickCounter { get; private set; }

        public void Tick(int tick)
        {
            TickCounter = tick;
        }
    }
}
