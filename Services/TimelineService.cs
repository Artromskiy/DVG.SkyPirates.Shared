using Arch.Core;
using DVG.SkyPirates.Shared.IServices;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;

namespace DVG.SkyPirates.Shared.Services
{
    public class TimelineService : ITimelineService
    {
        private const int RollbackInitTicks = 50;
        // last saved state
        public int CurrentTick { get; private set; } = -1;
        // tick before not applied command
        private int? _rollbackTo;

        private readonly ICommandExecutorService _commandExecutorService;
        private readonly ITickableExecutorService _tickableExecutorService;
        private readonly IPreTickableExecutorService _preTickableExecutorService;
        private readonly IPostTickableExecutorService _postTickableExecutorService;

        private readonly IRollbackHistorySystem _rollbackHistorySystem;
        private readonly ISaveHistorySystem _saveHistorySystem;
        private readonly IDisposeSystem _disposeSystem;
        private readonly World _world;

        public TimelineService(ICommandExecutorService commandExecutorService, ITickableExecutorService tickableExecutorService, IPreTickableExecutorService preTickableExecutorService, IPostTickableExecutorService postTickableExecutorService, IRollbackHistorySystem rollbackHistorySystem, ISaveHistorySystem saveHistorySystem, IDisposeSystem disposeSystem, World world)
        {
            _commandExecutorService = commandExecutorService;
            _tickableExecutorService = tickableExecutorService;
            _preTickableExecutorService = preTickableExecutorService;
            _postTickableExecutorService = postTickableExecutorService;
            _rollbackHistorySystem = rollbackHistorySystem;
            _saveHistorySystem = saveHistorySystem;
            _disposeSystem = disposeSystem;
            _world = world;
        }

        public void Tick()
        {
            _preTickableExecutorService.Tick(CurrentTick, Constants.TickTime);

            if (_rollbackTo.HasValue)
            {
                _rollbackHistorySystem.Tick(_rollbackTo.Value, Constants.TickTime);
            }

            var fromTick = Maths.Min(_rollbackTo ?? CurrentTick, CurrentTick) + 1;
            var toTick = CurrentTick + 1;
            for (int i = fromTick; i <= toTick; i++)
            {
                _commandExecutorService.Tick(i, Constants.TickTime);
                _tickableExecutorService.Tick(i, Constants.TickTime);
                _disposeSystem.Tick(CurrentTick, Constants.TickTime);
                _saveHistorySystem.Tick(i, Constants.TickTime);
            }
            CurrentTick = toTick;
            _rollbackTo = null;

            _postTickableExecutorService.Tick(CurrentTick, Constants.TickTime);
        }
    }
}