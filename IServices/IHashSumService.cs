using DVG.SkyPirates.Shared.Data;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.IServices
{
    public interface IHashSumService
    {
        (string hash, WorldData snapshot, int version) GetSnapshot(int tick);
        Dictionary<int, (string hash, WorldData snapshot, int version)> GetSnapshots();
    }
}
