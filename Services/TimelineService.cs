﻿using DVG.Core;
using DVG.Core.Commands;
using DVG.Core.Mementos;
using DVG.SkyPirates.Shared.IServices;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Services
{
    public class TimelineService : ITimelineService
    {
        public int CurrentTick { get; private set; }

        private readonly List<GenericCollection> _commands = new List<GenericCollection>();
        private readonly List<GenericCollection> _mementos = new List<GenericCollection>();
        private readonly List<float> _ticks = new List<float>();

        private readonly ICommandRecieveService _commandRecieveService;
        private readonly ICommandExecutorService _commandExecutorService;
        private readonly IEntitiesService _entitiesService;
        private readonly IOwnershipService _ownershipService;

        private int oldestCommandTick;

        public TimelineService(
            IEntitiesService entitiesService,
            IOwnershipService ownershipService,

            ICommandRecieveService commandRecieveService,
            ICommandExecutorService commandExecutorService)
        {
            _entitiesService = entitiesService;
            _ownershipService = ownershipService;
            _commandRecieveService = commandRecieveService;
            _commandExecutorService = commandExecutorService;

            _mementos.Add(new GenericCollection());
            CommandIds.ForEachData(new RegisterRecieverAction(this));
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

            MementoIds.ForEachData(new ApplyMementosAction(this, stateToApply));

            //_entitiesService.RemoveAllExcept(_currentEntityIds);
            //_ownershipService.RemoveAllExcept(_currentEntityIds);

            // Apply old memento, remove unused => (apply command => update => save memento) repeat
            for (int i = oldestCommandTick; i <= CurrentTick; i++)
            {
                CommandIds.ForEachData(new ApplyCommandAction(this, CommandsAtTick(i)));

                foreach (var entityId in _entitiesService.GetEntityIds())
                    if (_entitiesService.TryGetEntity<ITickable>(entityId, out var entity))
                        entity.Tick(_ticks[i]);

                MementoIds.ForEachData(new SaveMementosAction(this, _mementos[i]));
            }
            CurrentTick++;
            oldestCommandTick = CurrentTick;
        }

        private void RegisterReciever<T>()
            where T : ICommandData
        {
            _commandRecieveService.RegisterReciever<T>(AddCommand);
        }

        private void ApplyMementos<T>(GenericCollection mementos)
            where T : IMementoData
        {
            var genericMementos = mementos.GetCollection<Memento<T>>();
            if (genericMementos == null)
                return;

            foreach (var item in genericMementos)
                if (_entitiesService.TryGetEntity<IMementoable<T>>(item.EntityId, out var entity))
                    entity.SetMemento(item.Data);
        }

        private void SaveMementos<T>(GenericCollection mementos)
            where T : IMementoData
        {
            mementos.Clear<Memento<T>>();

            foreach (var entityId in _entitiesService.GetEntityIds())
                if (_entitiesService.TryGetEntity<IMementoable<T>>(entityId, out var entity))
                    mementos.Add(new Memento<T>(0, entityId, entity.GetMemento()));

            mementos.Trim<Memento<T>>();
        }

        private void ApplyCommand<T>(GenericCollection commands)
            where T : ICommandData
        {
            var genericCommands = commands.GetCollection<Command<T>>();
            if (genericCommands == null)
                return;

            foreach (var item in genericCommands)
                _commandExecutorService.Execute(item);
        }
        private readonly struct RegisterRecieverAction : IGenericAction<ICommandData>
        {
            private readonly TimelineService _service;

            public RegisterRecieverAction(TimelineService service)
            {
                _service = service;
            }

            public void Invoke<T>() where T : ICommandData => _service.RegisterReciever<T>();
        }

        private readonly struct ApplyMementosAction : IGenericAction<IMementoData>
        {
            private readonly TimelineService _service;
            private readonly GenericCollection _mementos;

            public ApplyMementosAction(TimelineService service, GenericCollection mementos)
            {
                _service = service;
                _mementos = mementos;
            }

            public void Invoke<T>() where T : IMementoData => _service.ApplyMementos<T>(_mementos);
        }

        private readonly struct SaveMementosAction : IGenericAction<IMementoData>
        {
            private readonly TimelineService _service;
            private readonly GenericCollection _mementos;

            public SaveMementosAction(TimelineService service, GenericCollection mementos)
            {
                _service = service;
                _mementos = mementos;
            }

            public void Invoke<T>() where T : IMementoData => _service.SaveMementos<T>(_mementos);
        }

        private readonly struct ApplyCommandAction : IGenericAction<ICommandData>
        {
            private readonly TimelineService _service;
            private readonly GenericCollection _commands;

            public ApplyCommandAction(TimelineService service, GenericCollection commands)
            {
                _service = service;
                _commands = commands;
            }

            public readonly void Invoke<T>() where T : ICommandData => _service.ApplyCommand<T>(_commands);
        }

    }
}
