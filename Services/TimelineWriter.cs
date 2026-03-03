using DVG.Components;
using DVG.SkyPirates.Shared.Data;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Services
{
    public class TimelineWriter : ITickableExecutor
    {
        private readonly IHistorySystem _historySystem;
        private readonly Dictionary<int, WorldData> _timeline = new();

        public TimelineWriter(IHistorySystem historySystem)
        {
            _historySystem = historySystem;
        }

        public void Tick(int tick)
        {
            var snapshot = _historySystem.GetSnapshot(tick - Constants.MaxHistoryTicks + 1);
            var trim = new TrimWorldData(snapshot);
            HistoryComponentsRegistry.ForEachData(ref trim);
            _timeline[tick] = snapshot;
        }

        public Dictionary<int, WorldData> GetSnapshots() => _timeline;

        private readonly struct TrimWorldData : IStructGenericAction
        {
            private readonly WorldData _data;

            public TrimWorldData(WorldData data)
            {
                _data = data;
            }

            public readonly void Invoke<T>() where T : struct
            {
                _data.Get<T>().TrimExcess();
            }
        }
    }
}
