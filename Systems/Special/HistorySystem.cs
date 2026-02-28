using Arch.Core;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;

namespace DVG.SkyPirates.Shared.Systems.Special
{
    internal sealed class HistorySystem : IHistorySystem
    {
        private readonly SaveHistorySystem _save;
        private readonly RollbackHistorySystem _rollBack;

        public HistorySystem(World world)
        {
            _save = new SaveHistorySystem(world);
            _rollBack = new RollbackHistorySystem(world);
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
    }
}
