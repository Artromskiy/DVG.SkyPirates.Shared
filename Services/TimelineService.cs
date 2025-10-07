using Arch.Core;
using DVG.Core;
using DVG.Core.Commands;
using DVG.SkyPirates.Shared.Commands;
using DVG.SkyPirates.Shared.Data;
using DVG.SkyPirates.Shared.IServices;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;
using DVG.SkyPirates.Shared.Systems.Special;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Services
{
    public class TimelineService : ITimelineService
    {
        private const int RollbackInitTicks = 50;
        // last saved state
        public int CurrentTick { get; private set; } = -1;
        // tick before not applied command
        private int? _rollbackTo;

        private readonly Dictionary<int, CommandCollection> _commands = new();

        private readonly ICommandRecieveService _commandRecieveService;
        private readonly ICommandExecutorService _commandExecutorService;
        private readonly ITickableExecutorService _tickableExecutorService;
        private readonly IPreTickableExecutorService _preTickableExecutorService;
        private readonly IPostTickableExecutorService _postTickableExecutorService;

        private readonly RollbackHistorySystem _rollbackHistorySystem;
        private readonly SaveHistorySystem _saveHistorySystem;
        private readonly DestructSystem _destructSystem;
        private readonly World _world;

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
            _world = world;

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
            var cmdRollback = tick - 1;
            GetCommands(tick).Add(command);

            _rollbackTo = cmdRollback < CurrentTick && 
                (!_rollbackTo.HasValue || cmdRollback < _rollbackTo.Value) ?
                cmdRollback :
                _rollbackTo;
        }

        public void RemoveCommand<T>(Command<T> command)
            where T : ICommandData
        {
            var tick = command.Tick;
            var cmdRollback = tick - 1;
            GetCommands(tick).Remove<T>(command.ClientId);

            _rollbackTo = cmdRollback < CurrentTick &&
                (!_rollbackTo.HasValue || cmdRollback < _rollbackTo.Value) ?
                cmdRollback :
                _rollbackTo;
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

            if (_rollbackTo.HasValue)
            {
                _rollbackHistorySystem.Tick(_rollbackTo.Value, Constants.TickTime);
            }
            CurrentTick++;
            var fromTick = Maths.Min(_rollbackTo ?? CurrentTick, CurrentTick) + 1;
            for (int i = fromTick; i <= CurrentTick + 1; i++)
            {
                _commandExecutorService.Execute(GetCommands(i));
                _tickableExecutorService.Tick(i, Constants.TickTime);
                _saveHistorySystem.Tick(i, Constants.TickTime);
            }
            _rollbackTo = null;

            _destructSystem.Tick(CurrentTick, Constants.TickTime);
            _postTickableExecutorService.Tick(CurrentTick, Constants.TickTime);
        }

        private List<CommandCollection> GetCommandsAfter(int tick)
        {
            List<CommandCollection> commands = new();
            foreach (var item in _commands)
                if (item.Key > tick)
                    commands.Add(item.Value);
            return commands;
        }

        public void Init(LoadWorldCommand timelineStart)
        {
            CurrentTick = timelineStart.CurrentTick - RollbackInitTicks;
            WorldDataSerializer.Deserialize(_world, timelineStart.WorldData);
            CommandsDataSerializer.Deserialize(this, timelineStart.CommandsData);
            for (int i = 0; i < RollbackInitTicks; i++)
            {
                Tick();
            }
        }

        public LoadWorldCommand GetIniter()
        {
            int targetTick = CurrentTick - RollbackInitTicks;
            _rollbackHistorySystem.Tick(targetTick, Constants.TickTime);
            var worldData = WorldDataSerializer.Serialize(_world);
            var commandsData = CommandsDataSerializer.Serialize(GetCommandsAfter(targetTick));
            _rollbackHistorySystem.Tick(CurrentTick, Constants.TickTime);
            return new LoadWorldCommand(worldData, commandsData, CurrentTick);
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