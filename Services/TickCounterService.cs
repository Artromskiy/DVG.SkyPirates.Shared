using DVG;
using DVG.SkyPirates.Client.IServices;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;

namespace Assets.Scripts.SkyPirates.Shared.Services
{
    public class TickCounterService : ITickCounterService, IPostTickableExecutor
    {
        public int TickCounter { get; private set; }

        public void Tick(int tick, fix deltaTime)
        {
            TickCounter = tick;
        }
    }
}
