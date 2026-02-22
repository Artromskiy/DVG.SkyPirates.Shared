using DVG.SkyPirates.Shared.IServices;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;

namespace DVG.SkyPirates.Shared.Services
{
    public class TimelineService : ITimelineService
    {
        // last saved state
        public int CurrentTick { get; private set; } = -1;
        // tick before not applied command
        public int DirtyTick { get; set; } = -1;

        private readonly ICommandExecutorService _commandExecutorService;
        private readonly ITickableExecutorService _tickableExecutorService;
        private readonly IPreTickableExecutorService _preTickableExecutorService;
        private readonly IPostTickableExecutorService _postTickableExecutorService;

        private readonly IRollbackHistorySystem _rollbackHistorySystem;
        private readonly ISaveHistorySystem _saveHistorySystem;
        private readonly IDisposeSystem _disposeSystem;

        public TimelineService(ICommandExecutorService commandExecutorService, ITickableExecutorService tickableExecutorService, IPreTickableExecutorService preTickableExecutorService, IPostTickableExecutorService postTickableExecutorService, IRollbackHistorySystem rollbackHistorySystem, ISaveHistorySystem saveHistorySystem, IDisposeSystem disposeSystem)
        {
            _commandExecutorService = commandExecutorService;
            _tickableExecutorService = tickableExecutorService;
            _preTickableExecutorService = preTickableExecutorService;
            _postTickableExecutorService = postTickableExecutorService;
            _rollbackHistorySystem = rollbackHistorySystem;
            _saveHistorySystem = saveHistorySystem;
            _disposeSystem = disposeSystem;
        }

        public void Tick()
        {
            var nextTick = CurrentTick + 1;
            _preTickableExecutorService.Tick(nextTick, Constants.TickTime);

            if (DirtyTick != CurrentTick)
            {
                GoTo(DirtyTick - 1);
            }

            var fromTick = CurrentTick + 1;
            for (int i = fromTick; i <= nextTick; i++)
                Tick(i);

            DirtyTick = CurrentTick;

            _postTickableExecutorService.Tick(nextTick, Constants.TickTime);
        }

        public void GoTo(int tick)
        {
            _rollbackHistorySystem.Tick(tick, Constants.TickTime);
            CurrentTick = tick;
        }

        private void Tick(int tick)
        {
            CurrentTick = tick;
            _commandExecutorService.Tick(CurrentTick, Constants.TickTime);
            _tickableExecutorService.Tick(CurrentTick, Constants.TickTime);
            _disposeSystem.Tick(CurrentTick, Constants.TickTime);
            _saveHistorySystem.Tick(CurrentTick, Constants.TickTime);
        }
    }
}