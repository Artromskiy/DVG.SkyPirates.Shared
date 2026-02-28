using DVG.SkyPirates.Shared.IServices;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;

namespace DVG.SkyPirates.Shared.Services
{
    public class TimelineService : ITimelineService
    {
        // last saved state
        public int CurrentTick { get; set; } = -1;
        // tick before not applied command
        public int DirtyTick { get; set; } = int.MaxValue;

        private readonly ICommandExecutorService _commandExecutorService;
        private readonly IDeltaTickableService<IDeltaTickableExecutor> _deltaTickableService;

        private readonly IHistorySystem _historySystem;
        private readonly IDisposeSystem _disposeSystem;

        public TimelineService(ICommandExecutorService commandExecutorService, IDeltaTickableService<IDeltaTickableExecutor> deltaTickableService, IHistorySystem historySystem, IDisposeSystem disposeSystem)
        {
            _commandExecutorService = commandExecutorService;
            _deltaTickableService = deltaTickableService;
            _historySystem = historySystem;
            _disposeSystem = disposeSystem;
        }

        public void Tick(int tick)
        {
            if (DirtyTick <= CurrentTick)
            {
                _historySystem.Rollback(DirtyTick - 1);
                CurrentTick = DirtyTick - 1;
            }

            var fromTick = CurrentTick + 1;
            for (int i = fromTick; i <= tick; i++)
            {
                CurrentTick = i;
                _commandExecutorService.Tick(CurrentTick);
                _deltaTickableService.Tick(CurrentTick, Constants.TickTime);
                _disposeSystem.Tick(CurrentTick);
                _historySystem.Save(CurrentTick);
            }
            DirtyTick = int.MaxValue;
        }

        public void GoTo(int tick)
        {
            _historySystem.GoTo(tick);
            CurrentTick = tick;
        }
    }
}