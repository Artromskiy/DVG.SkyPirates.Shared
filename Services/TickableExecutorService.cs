using DVG.SkyPirates.Shared.IServices;
using System.Collections.Generic;
using System.Linq;

namespace DVG.SkyPirates.Shared.Services
{
    public class TickableExecutorService : ITickableExecutorService
    {
        private readonly ITickableExecutor[] _executors;

        public TickableExecutorService(IEnumerable<ITickableExecutor> executors)
        {
            _executors = executors.ToArray();
        }

        public void Tick(fix deltaTime)
        {
            foreach (var item in _executors)
                item.Tick(deltaTime);
        }
    }
}
