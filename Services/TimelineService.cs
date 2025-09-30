using Arch.Core;
using DVG.Core;
using DVG.Core.Commands;
using DVG.SkyPirates.Shared.IServices;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;
using DVG.SkyPirates.Shared.Systems.Special;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Services
{
    public class TimelineService : ITimelineService
    {
        public int CurrentTick { get; set; }
        private int? _dirtyTick;

        private readonly Dictionary<int, CommandCollection> _commands = new Dictionary<int, CommandCollection>();

        private readonly ICommandRecieveService _commandRecieveService;
        private readonly ICommandExecutorService _commandExecutorService;
        private readonly ITickableExecutorService _tickableExecutorService;
        private readonly IPreTickableExecutorService _preTickableExecutorService;
        private readonly IPostTickableExecutorService _postTickableExecutorService;

        private readonly RollbackHistorySystem _rollbackHistorySystem;
        private readonly SaveHistorySystem _saveHistorySystem;
        private readonly DestructSystem _destructSystem;

        public TimelineService(
            World world,
            ICommandRecieveService commandRecieveService,
            ICommandExecutorService commandExecutorService,
            ITickableExecutorService tickableExecutorService,
            IPreTickableExecutorService preTickableExecutorService,
            IPostTickableExecutorService postTickableExecutorService)
        {
            _rollbackHistorySystem = new(world);
            _saveHistorySystem = new(world);
            _destructSystem = new(world);

            _commandRecieveService = commandRecieveService;
            _commandExecutorService = commandExecutorService;
            _tickableExecutorService = tickableExecutorService;
            _preTickableExecutorService = preTickableExecutorService;
            _postTickableExecutorService = postTickableExecutorService;

            var action = new RegisterRecieverAction(this, _commandRecieveService);
            CommandIds.ForEachData(ref action);
        }

        public void AddCommand<T>(Command<T> command)
            where T : ICommandData
        {
            var tick = command.Tick;

            var prevDirtyTick = _dirtyTick ?? tick;
            _dirtyTick = Maths.Min(tick, prevDirtyTick);

            GetCommands(tick).Add(command);
        }

        public void RemoveCommand<T>(Command<T> command)
            where T : ICommandData
        {
            var tick = command.Tick;

            var prevDirtyTick = _dirtyTick ?? tick;
            _dirtyTick = Maths.Min(tick, prevDirtyTick);

            GetCommands(tick).Remove<T>(command.ClientId);
        }

        private CommandCollection GetCommands(int tick)
        {
            if (!_commands.TryGetValue(tick, out var commandCollection))
            {
                _commands[tick] = commandCollection = new CommandCollection();
            }
            return commandCollection;
        }

        public void Tick()
        {
            _preTickableExecutorService.Tick(CurrentTick, Constants.TickTime);

            if (_dirtyTick.HasValue && _dirtyTick < CurrentTick)
            {
                int tickToGo = _dirtyTick.Value - 1;
                _rollbackHistorySystem.Tick(tickToGo, Constants.TickTime);
            }
            var fromTick = Maths.Min(_dirtyTick ?? CurrentTick, CurrentTick);
            for (int i = fromTick; i <= CurrentTick; i++)
            {
                _commandExecutorService.Execute(GetCommands(i));
                _tickableExecutorService.Tick(i, Constants.TickTime);
                _saveHistorySystem.Tick(i, Constants.TickTime);
            }
            _dirtyTick = null;

            _destructSystem.Tick(CurrentTick, Constants.TickTime);
            _postTickableExecutorService.Tick(CurrentTick++, Constants.TickTime);
        }

        private readonly struct RegisterRecieverAction : IGenericAction<ICommandData>
        {
            private readonly ITimelineService _timelineService;
            private readonly ICommandRecieveService _recieveService;

            public RegisterRecieverAction(ITimelineService timelineService, ICommandRecieveService recieveService)
            {
                _timelineService = timelineService;
                _recieveService = recieveService;
            }

            public void Invoke<T>() where T : ICommandData
            {
                _recieveService.RegisterReciever<T>(_timelineService.AddCommand);
            }
        }
    }
}