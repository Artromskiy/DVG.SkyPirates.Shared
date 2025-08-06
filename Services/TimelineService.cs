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
        public float TickTime { get; set; }

        private readonly Dictionary<int, GenericCollection> _commands = new Dictionary<int, GenericCollection>();
        private readonly Dictionary<int, GenericCollection> _mementos = new Dictionary<int, GenericCollection>();

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

            CommandIds.ForEachData(new RegisterRecieverAction(this, _commandRecieveService));
        }

        private GenericCollection GetCommands(int tick)
        {
            if(!_commands.TryGetValue(tick, out var genericCollection))
            {
                _commands[tick] = genericCollection = new GenericCollection();
            }
            return genericCollection;
        }

        private GenericCollection GetMementos(int tick)
        {
            if (!_mementos.TryGetValue(tick, out var genericCollection))
            {
                _mementos[tick] = genericCollection = new GenericCollection();
            }
            return genericCollection;
        }

        public void AddCommand<T>(Command<T> command)
            where T : ICommandData
        {
            var tick = command.Tick;
            oldestCommandTick = Maths.Min(tick, oldestCommandTick);
            GetCommands(tick).Add(command);
        }

        public void RemoveCommand(int clientId, int commandId)
        {
            //throw new NotImplementedException();
        }

        public void Tick()
        {
            int tickToGo = oldestCommandTick - 1;
            var stateToApply = GetMementos(tickToGo);
            _entitiesService.CurrentTick = tickToGo;

            MementoIds.ForEachData(new ApplyMementosAction(_entitiesService, stateToApply));

            // (copy entities, apply command => update => save memento) repeat
            for (int i = oldestCommandTick; i <= CurrentTick; i++)
            {
                var entities = _entitiesService.GetEntities(i);
                var prevEntities = _entitiesService.GetEntities(i - 1);
                entities.Clear();
                foreach (var (id, entity) in prevEntities)
                    entities.Add(id, entity);

                _entitiesService.CurrentTick = i;

                CommandIds.ForEachData(new ApplyCommandAction(_commandExecutorService, GetCommands(i)));

                foreach (var (id, obj) in _entitiesService.GetEntities(i))
                    if (obj is ITickable entity)
                        entity.Tick(TickTime);

                MementoIds.ForEachData(new SaveMementosAction(_entitiesService, GetMementos(i)));
            }
            CurrentTick++;
            oldestCommandTick = CurrentTick;
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

        private readonly struct ApplyMementosAction : IGenericAction<IMementoData>
        {
            private readonly IEntitiesService _entities;
            private readonly GenericCollection _mementos;

            public ApplyMementosAction(IEntitiesService entities, GenericCollection mementos)
            {
                _entities = entities;
                _mementos = mementos;
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
            private readonly IEntitiesService _entities;
            private readonly GenericCollection _mementos;

            public SaveMementosAction(IEntitiesService entities, GenericCollection mementos)
            {
                _entities = entities;
                _mementos = mementos;
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
