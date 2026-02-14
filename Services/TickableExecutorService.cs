using DVG.SkyPirates.Shared.IServices.TickableExecutors;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Profiling;

namespace DVG.SkyPirates.Shared.Services
{
    public class TickableExecutorService : ITickableExecutorService
    {
        private readonly ITickableExecutor[] _executors;

        public TickableExecutorService(IEnumerable<ITickableExecutor> executors)
        {
            _executors = executors.ToArray();
        }

        public void Tick(int tick, fix deltaTime)
        {
            for (int i = 0; i < _executors.Length; i++)
            {
                ITickableExecutor item = _executors[i];
                Profiler.BeginSample(item.ToString());
                item.Tick(tick, deltaTime);
                Profiler.EndSample();
            }
        }
    }
}
