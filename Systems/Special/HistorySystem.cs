using Arch.Core;
using DVG.SkyPirates.Shared.Data;
using DVG.SkyPirates.Shared.IFactories;
using DVG.SkyPirates.Shared.IServices;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;

namespace DVG.SkyPirates.Shared.Systems.Special
{
    internal sealed class HistorySystem : IHistorySystem
    {
        private readonly SaveHistorySystem _save;
        private readonly RollbackHistorySystem _rollBack;
        private readonly SnapshotHistorySystem _snapshot;

        public HistorySystem(World world, IEntityFactory entityFactory, IEntityRegistry entityRegistry)
        {
            _save = new SaveHistorySystem(world);
            _rollBack = new RollbackHistorySystem(world);
            _snapshot = new SnapshotHistorySystem(world, entityFactory, entityRegistry);
        }

        public void GoTo(int tick)
        {
            _rollBack.GoTo(tick);
        }

        public void Rollback(int tick)
        {
            _rollBack.RollBack(tick);
        }

        public void Save(int tick)
        {
            _save.Save(tick);
        }

        public void ApplySnapshot(WorldData snapshot)
        {
            _snapshot.ApplySnapshot(snapshot);
        }

        public WorldData GetSnapshot(int tick)
        {
            return _snapshot.GetSnapshot(tick);
        }
    }
}
