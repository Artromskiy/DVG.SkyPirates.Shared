using DVG.SkyPirates.Shared.Data;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.IServices
{
    public interface IHashSumService : ITickableExecutor
    {
        (string hash, WorldData snapshot, int version) GetSnapshot(int tick);
        Dictionary<int, (string hash, WorldData snapshot, int version)> GetSnapshots();
    }
}
