using DVG.Core;
using DVG.Core.Commands;
using DVG.SkyPirates.Shared.IServices;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Services
{
    public class TimelineService : ITimelineService
    {
        private static readonly fix _tickTime = (fix)1 / Constants.TicksPerSecond;
        public int CurrentTick { get; private set; }

        private readonly Dictionary<int, GenericCollection> _commands = new Dictionary<int, GenericCollection>();
        private int _oldestCommandTick;

        private readonly ICommandRecieveService _commandRecieveService;
        private readonly ICommandExecutorService _commandExecutorService;
        private readonly ITickableExecutorService _tickableExecutorService;
        private readonly IPreTickableExecutorService _preTickableExecutorService;
        private readonly IPostTickableExecutorService _postTickableExecutorService;

        public TimelineService(
            ICommandRecieveService commandRecieveService,
            ICommandExecutorService commandExecutorService,
            ITickableExecutorService tickableExecutorService,
            IPreTickableExecutorService preTickableExecutorService,
            IPostTickableExecutorService postTickableExecutorService)
        {
            _commandRecieveService = commandRecieveService;
            _commandExecutorService = commandExecutorService;
            _tickableExecutorService = tickableExecutorService;
            _preTickableExecutorService = preTickableExecutorService;
            _postTickableExecutorService = postTickableExecutorService;

            CommandIds.ForEachData(new RegisterRecieverAction(this, _commandRecieveService));
        }

        public void AddCommand<T>(Command<T> command)
            where T : ICommandData
        {
            var tick = command.Tick;
            _oldestCommandTick = Maths.Min(tick, _oldestCommandTick);
            GetCommands(tick).Add(command);
        }

        public void RemoveCommand<T>(Command<T> command)
            where T : ICommandData
        {
            var tick = command.Tick;
            _oldestCommandTick = Maths.Min(tick, _oldestCommandTick);
            GetCommands(tick).Remove<Command<T>>(c => c.ClientId == command.ClientId && c.CommandId == command.CommandId);
        }

        private GenericCollection GetCommands(int tick)
        {
            if (!_commands.TryGetValue(tick, out var genericCollection))
            {
                _commands[tick] = genericCollection = new GenericCollection();
            }
            return genericCollection;
        }

        public void Tick()
        {
            int tickToGo = _oldestCommandTick - 1;
            _preTickableExecutorService.Tick(tickToGo, _tickTime);

            for (int i = _oldestCommandTick; i <= CurrentTick; i++)
            {
                CommandIds.ForEachData(new ApplyCommandAction(_commandExecutorService, GetCommands(i)));
                _tickableExecutorService.Tick(i, _tickTime);
            }
            _postTickableExecutorService.Tick(CurrentTick, _tickTime);

            CurrentTick++;
            _oldestCommandTick = CurrentTick;
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

        private readonly struct ApplyCommandAction : IGenericAction<ICommandData>
        {
            private readonly ICommandExecutorService _executor;
            private readonly GenericCollection _commands;

            public ApplyCommandAction(ICommandExecutorService executor, GenericCollection commands)
            {
                _executor = executor;
                _commands = commands;
            }

            public readonly void Invoke<T>() where T : ICommandData
            {
                var genericCommands = _commands.GetCollection<Command<T>>();
                if (genericCommands == null)
                    return;

                foreach (var item in genericCommands)
                    _executor.Execute(item);
            }
        }
    }
}
