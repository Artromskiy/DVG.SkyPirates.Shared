namespace DVG.SkyPirates.Shared.IServices
{
    public interface ITimelineService
    {
        int CurrentTick { get; set; }
        int DirtyTick { get; set; }
        void TickTo(int targetTick);
        void GoTo(int tick);
    }
}
