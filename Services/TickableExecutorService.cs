using DVG.SkyPirates.Shared.IServices.TickableExecutors;
using System.Collections.Generic;
using System.Diagnostics;
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

        public void Tick(int tick, fix deltaTime)
        {
            Stopwatch sw = new Stopwatch();
            Stopwatch sw2 = new Stopwatch();
            sw2.Start();
            foreach (var item in _executors)
            {
                sw.Restart();
                item.Tick(tick, deltaTime);
                sw.Stop();
                //Console.WriteLine($"{item.GetType().Name}: {sw.Elapsed.TotalMilliseconds:0.000}");
            }
            sw2.Stop();
            //Console.WriteLine($"Total: {sw2.Elapsed.TotalMilliseconds:0.000}");
        }
    }
}
