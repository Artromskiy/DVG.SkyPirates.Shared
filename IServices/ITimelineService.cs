using DVG.Core;

namespace DVG.SkyPirates.Shared.IServices
{
    public interface ITimelineService
    {
        int CurrentTick { get; set; }
        void AddCommand<T>(Command<T> command) where T : ICommandData;
        void RemoveCommand<T>(Command<T> command) where T : ICommandData;
        void Tick();
    }
}
