using DVG.SkyPirates.Shared.IServices.TickableExecutors;
using System.Collections.Generic;
using System.Linq;

namespace DVG.SkyPirates.Shared.Services
{
    public class PostTickableExecutorService: IPostTickableExecutorService
    {
        private readonly IPostTickableExecutor[] _executors;

        public PostTickableExecutorService(IEnumerable<IPostTickableExecutor> executors)
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
