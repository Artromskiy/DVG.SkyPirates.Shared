using DVG.Core;
using DVG.Core.Commands;
using DVG.Core.Mementos;
using DVG.SkyPirates.Shared.IServices;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Services
{
    public class TimelineService : ITimelineService
    {
        public int CurrentTick { get; private set; }
        private int _newEntityId;

        private readonly List<GenericCollection> _commands = new List<GenericCollection>();
        private readonly List<GenericCollection> _mementos = new List<GenericCollection>();
        private readonly List<float> _ticks = new List<float>();

        private readonly ICommandRecieveService _commandRecieveService;
        private readonly ICommandExecutorService _commandExecutorService;
        private readonly IEntitiesService _entitiesService;

        private int oldestCommandTick;

        public TimelineService(
            ICommandRecieveService commandRecieveService,
            ICommandExecutorService commandExecutorService,
            IEntitiesService entitiesService)
        {
            _commandRecieveService = commandRecieveService;
            _commandExecutorService = commandExecutorService;
            _entitiesService = entitiesService;

            _mementos.Add(new GenericCollection());
            CommandIds.ForEachData(new RegisterRecieverAction(this, _commandRecieveService));
        }

        private GenericCollection CommandsAtTick(int tick)
        {
            int delta = Maths.Max(tick - (_commands.Count - 1), 0);
            for (int i = 0; i < delta; i++)
                _commands.Add(new GenericCollection());
            return _commands[tick];
        }


        public void AddCommand<T>(Command<T> command)
            where T : ICommandData
        {
            var tick = command.Tick;
            oldestCommandTick = Maths.Min(tick, oldestCommandTick);
            CommandsAtTick(tick).Add(command);
        }

        public void RemoveCommand(int clientId, int commandId)
        {
            //throw new NotImplementedException();
        }

        public void Tick(float deltaTime)
        {
            _mementos.Add(new GenericCollection());
            _ticks.Add(deltaTime);

            int tickToGo = oldestCommandTick - 1;
            var stateToApply = tickToGo < 0 ? new GenericCollection() : _mementos[tickToGo];
            _entitiesService.CurrentTick = tickToGo;

            MementoIds.ForEachData(new ApplyMementosAction(stateToApply, _entitiesService));

            // (copy entities, apply command => update => save memento) repeat
            for (int i = oldestCommandTick; i <= CurrentTick; i++)
            {
                _entitiesService.CurrentTick = i;
                _entitiesService.CopyPreviousEntities();

                CommandIds.ForEachData(new ApplyCommandAction(_commandExecutorService, CommandsAtTick(i)));

                foreach (var entityId in _entitiesService.GetEntityIds())
                    if (_entitiesService.TryGetEntity<ITickable>(entityId, out var entity))
                        entity.Tick(_ticks[i]);

                MementoIds.ForEachData(new SaveMementosAction(_mementos[i], _entitiesService));
            }
            CurrentTick++;
            oldestCommandTick = CurrentTick;
        }

        public int NewEntityId() => ++_newEntityId;

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

        private readonly struct ApplyMementosAction : IGenericAction<IMementoData>
        {
            private readonly GenericCollection _mementos;
            private readonly IEntitiesService _entities;

            public ApplyMementosAction(GenericCollection mementos, IEntitiesService entities)
            {
                _mementos = mementos;
                _entities = entities;
            }

            public void Invoke<T>() where T : IMementoData
            {
                var genericMementos = _mementos.GetCollection<Memento<T>>();
                if (genericMementos == null)
                    return;

                foreach (var item in genericMementos)
                    if (_entities.TryGetEntity<IMementoable<T>>(item.EntityId, out var entity))
                        entity.SetMemento(item.Data);
            }
        }

        private readonly struct SaveMementosAction : IGenericAction<IMementoData>
        {
            private readonly GenericCollection _mementos;
            private readonly IEntitiesService _entities;

            public SaveMementosAction(GenericCollection mementos, IEntitiesService entities)
            {
                _mementos = mementos;
                _entities = entities;
            }

            public void Invoke<T>() where T : IMementoData
            {
                _mementos.Clear<Memento<T>>();

                foreach (var entityId in _entities.GetEntityIds())
                    if (_entities.TryGetEntity<IMementoable<T>>(entityId, out var entity))
                        _mementos.Add(new Memento<T>(0, entityId, entity.GetMemento()));

                _mementos.Trim<Memento<T>>();
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
