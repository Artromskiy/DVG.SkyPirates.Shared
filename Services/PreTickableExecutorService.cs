using DVG.SkyPirates.Shared.IServices.TickableExecutors;
using System.Collections.Generic;
using System.Linq;

namespace DVG.SkyPirates.Shared.Services
{
    public class PreTickableExecutorService : IPreTickableExecutorService
    {
        private readonly IPreTickableExecutor[] _executors;

        public PreTickableExecutorService(IEnumerable<IPreTickableExecutor> executors)
        {
            _executors = executors.ToArray();
        }

        public void Tick(int tick, fix deltaTime)
        {
            foreach (var item in _executors)
                item.Tick(tick, deltaTime);
        }
    }
}
