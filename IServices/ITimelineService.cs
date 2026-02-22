namespace DVG.SkyPirates.Shared.IServices
{
    public interface ITimelineService
    {
        int CurrentTick { get; }
        int DirtyTick { get; set; }
        void Tick();
        void GoTo(int tick);
    }
}
