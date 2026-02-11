using DVG.Commands;
using DVG.Core;
using DVG.SkyPirates.Shared.Commands;

namespace DVG.SkyPirates.Shared.IServices
{
    public interface ITimelineService
    {
        int CurrentTick { get; }
        void Init(LoadWorldCommand timelineStart);
        LoadWorldCommand GetIniter();
        void AddCommand<T>(Command<T> command) where T : ICommandData;
        void RemoveCommand<T>(Command<T> command) where T : ICommandData;
        void Tick();
    }
}
