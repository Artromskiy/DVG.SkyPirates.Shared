namespace DVG.SkyPirates.Shared.IServices
{
    public interface ITimelineService
    {
        int CurrentTick { get; set; }
        int DirtyTick { get; set; }
        void Tick(int tick);
        void GoTo(int tick);
    }
}
