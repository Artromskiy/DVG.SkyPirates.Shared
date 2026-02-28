namespace DVG.SkyPirates.Shared.IServices.TickableExecutors
{
    public interface IHistorySystem
    {
        void Save(int tick);
        void GoTo(int tick);
        void Rollback(int tick);
    }
}
