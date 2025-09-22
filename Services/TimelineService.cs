using DVG.Core;
using DVG.Core.Commands;
using DVG.SkyPirates.Shared.IServices;
using DVG.SkyPirates.Shared.IServices.TickableExecutors;
using DVG.SkyPirates.Shared.Systems.HistorySystems;
using System;
using System.Collections.Generic;
using System.Text;

namespace DVG.SkyPirates.Shared.Services
{
    public class TimelineService : ITimelineService
    {
        private static readonly fix _tickTime = (fix)1 / Constants.TicksPerSecond;
        public int CurrentTick { get; private set; }

        private readonly Dictionary<int, CommandCollection> _commands = new Dictionary<int, CommandCollection>();
        private int? _notAppliedTick;
        private readonly LogHashSumSystem _logHashSumSystem;
        private readonly ICommandRecieveService _commandRecieveService;
        private readonly ICommandExecutorService _commandExecutorService;
        private readonly ITickableExecutorService _tickableExecutorService;
        private readonly IPreTickableExecutorService _preTickableExecutorService;
        private readonly IPostTickableExecutorService _postTickableExecutorService;

        public TimelineService(
            LogHashSumSystem logHashSumSystem,
            ICommandRecieveService commandRecieveService,
            ICommandExecutorService commandExecutorService,
            ITickableExecutorService tickableExecutorService,
            IPreTickableExecutorService preTickableExecutorService,
            IPostTickableExecutorService postTickableExecutorService)
        {
            _logHashSumSystem = logHashSumSystem;
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
            var prevNotAppliedTick = _notAppliedTick ?? tick;
            _notAppliedTick = Maths.Min(tick, prevNotAppliedTick);
            GetCommands(tick).Add(command);
        }

        public void RemoveCommand<T>(Command<T> command)
            where T : ICommandData
        {
            var tick = command.Tick;

            var prevNotAppliedTick = _notAppliedTick ?? tick;
            _notAppliedTick = Maths.Min(tick, prevNotAppliedTick);
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
            if (_notAppliedTick.HasValue)
            {
                int tickToGo = _notAppliedTick.Value - 1;
                _preTickableExecutorService.Tick(tickToGo, _tickTime);
                Console.WriteLine($"Rollback: {tickToGo}, Hash: {_logHashSumSystem.GetHashSum()}");
            }
            var fromTick = _notAppliedTick ?? CurrentTick;
            StringBuilder sb = new StringBuilder().AppendLine();
            for (int i = fromTick; i <= CurrentTick; i++)
            {
                sb.AppendLine($"Internal Before command: {i}, Hash{_logHashSumSystem.GetHashSum()}");
                _commandExecutorService.Execute(GetCommands(i));
                sb.AppendLine($"Internal Before tick: {i}, Hash{_logHashSumSystem.GetHashSum()}");
                _tickableExecutorService.Tick(i, _tickTime);
                sb.AppendLine($"Internal After tick: {i}, Hash{_logHashSumSystem.GetHashSum()}");
            }
            Console.WriteLine($"TickStep: {CurrentTick}, {sb}");

            int cmdSum = 0;
            foreach (var item in _commands)
            {
                if (item.Key <= _notAppliedTick)
                {
                    var countAction = new GetSumAction(_commands[item.Key]);
                    CommandIds.ForEachData(ref countAction);
                    cmdSum += countAction.Sum;
                }
            }
            Console.WriteLine($"Tick: {CurrentTick}, CmdCount: {cmdSum}");

            // Set oldest cmd tick now
            // we can recieve commands in post tick
            // and they will not be applied, because oldest command tick update
            int postTick = CurrentTick++;
            _notAppliedTick = CurrentTick;

            _postTickableExecutorService.Tick(postTick, _tickTime);
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

        private struct GetHashAction : IGenericAction<ICommandData>
        {
            public int Hash;
            private readonly CommandCollection _commandCollection;

            public GetHashAction(CommandCollection commandCollection)
            {
                Hash = 0;
                _commandCollection = commandCollection;
            }

            public void Invoke<T>() where T : ICommandData
            {
                var coll = _commandCollection.GetCollection<T>();
                if (coll != null)
                {
                    foreach (var item in coll)
                    {
                        Hash += item.ClientId + item.CommandId + item.EntityId + item.Tick;
                    }
                }
                else
                {
                    Hash -= 100;
                }
            }
        }

        private struct GetSumAction : IGenericAction<ICommandData>
        {
            public int Sum;
            private readonly CommandCollection _commandCollection;

            public GetSumAction(CommandCollection commandCollection)
            {
                Sum = 0;
                _commandCollection = commandCollection;
            }

            public void Invoke<T>() where T : ICommandData
            {
                var coll = _commandCollection.GetCollection<T>();
                if (coll != null)
                    Sum += coll.Count;
            }
        }
    }
}