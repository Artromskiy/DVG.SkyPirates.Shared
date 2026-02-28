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
        private readonly ITickableExecutorService _tickableExecutorService;
        private readonly IPreTickableExecutorService _preTickableExecutorService;
        private readonly IPostTickableExecutorService _postTickableExecutorService;

        private readonly IHistorySystem _historySystem;
        private readonly IDisposeSystem _disposeSystem;

        public TimelineService(ICommandExecutorService commandExecutorService, ITickableExecutorService tickableExecutorService, IPreTickableExecutorService preTickableExecutorService, IPostTickableExecutorService postTickableExecutorService, IHistorySystem historySystem, IDisposeSystem disposeSystem)
        {
            _commandExecutorService = commandExecutorService;
            _tickableExecutorService = tickableExecutorService;
            _preTickableExecutorService = preTickableExecutorService;
            _postTickableExecutorService = postTickableExecutorService;
            _historySystem = historySystem;
            _disposeSystem = disposeSystem;
        }

        public void TickTo(int targetTick)
        {
            _preTickableExecutorService.Tick(targetTick, Constants.TickTime);

            if (DirtyTick <= CurrentTick)
            {
                _historySystem.Rollback(DirtyTick - 1);
                CurrentTick = DirtyTick - 1;
            }

            var fromTick = CurrentTick + 1;
            for (int i = fromTick; i <= targetTick; i++)
                Tick(i);

            DirtyTick = int.MaxValue;

            _postTickableExecutorService.Tick(targetTick, Constants.TickTime);
        }

        public void GoTo(int tick)
        {
            _historySystem.GoTo(tick);
            CurrentTick = tick;
        }

        private void Tick(int tick)
        {
            CurrentTick = tick;
            _commandExecutorService.Tick(CurrentTick, Constants.TickTime);
            _tickableExecutorService.Tick(CurrentTick, Constants.TickTime);
            _disposeSystem.Tick(CurrentTick, Constants.TickTime);
            _historySystem.Save(CurrentTick);
        }
    }
}