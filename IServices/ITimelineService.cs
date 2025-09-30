using DVG.Core;
using DVG.SkyPirates.Shared.Commands;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.IServices
{
    public interface ITimelineService
    {
        int CurrentTick { get; set; }
        void Init(TimelineStartCommand timelineStart);
        TimelineStartCommand GetIniter();
        void AddCommand<T>(Command<T> command) where T : ICommandData;
        void RemoveCommand<T>(Command<T> command) where T : ICommandData;
        void Tick();
    }
}
