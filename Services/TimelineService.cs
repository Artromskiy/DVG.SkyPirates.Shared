using DVG.Core;
using DVG.Core.Commands;
using DVG.Core.Mementos;
using DVG.SkyPirates.Shared.IServices;
using System;
using System.Collections.Generic;

namespace DVG.SkyPirates.Shared.Services
{
    public class TimelineService : ITimelineService
    {
        public int CurrentTick { get; private set; }

        private readonly List<GenericCollection> _commands = new List<GenericCollection>();
        private readonly List<GenericCollection> _mementos = new List<GenericCollection>();
        private readonly HashSet<int> _entityIds = new HashSet<int>();

        private readonly IEntitiesService _entitiesService;
        private readonly IPlayerLoopSystem _playerLoopSystem;

        private int oldestCommandTick;

        public TimelineService(IEntitiesService entitiesService, IPlayerLoopSystem playerLoopSystem)
        {
            _entitiesService = entitiesService;
            _playerLoopSystem = playerLoopSystem;
        }

        public void AddCommand<T>(Command<T> command)
            where T : ICommandData
        {
            oldestCommandTick = Maths.Min(command.Tick, oldestCommandTick);
            _commands[command.CommandId].Add(command);
        }

        public void RemoveCommand(int clientId, int commandId)
        {
            throw new NotImplementedException();
        }

        public void Tick()
        {
            var stateToApply = _mementos[oldestCommandTick - 1];

            _entityIds.Clear();
            MementoIds.ForEachData(new ApplyMementosAction(this, stateToApply));
            _entitiesService.RemoveAllExcept(_entityIds);

            for (int i = oldestCommandTick; i < CurrentTick; i++)
            {
                CommandIds.ForEachData(new ApplyCommandAction(this, _commands[i]));
                _playerLoopSystem.Tick(1f / 10);
                MementoIds.ForEachData(new SaveMementosAction(this, _mementos[i]));
            }
                
            CurrentTick++;
            oldestCommandTick = CurrentTick;
        }

        private void ApplyMementos<T>(GenericCollection mementos)
            where T : IMementoData
        {
            var genericMementos = mementos.GetCollection<Memento<T>>();
            foreach (var item in genericMementos)
            {
                _entitiesService.TryGetEntity<IMementoable<T>>(item.EntityId, out var entity);
                entity.SetMemento(item.Data);
                _entityIds.Add(item.EntityId);
            }
        }

        private void SaveMementos<T>(GenericCollection mementos)
            where T : IMementoData
        {
            mementos.GetCollection<Memento<T>>().Clear();
            var entities = _entitiesService.GetEntities<IMementoable<T>>();
            foreach (var item in entities)
                mementos.Add(item.GetMemento());

        }

        private void ApplyCommand<T>(GenericCollection commands)
            where T : ICommandData
        {
            var genericCommands = commands.GetCollection<Command<T>>();
            foreach (var item in genericCommands)
            {
                _entitiesService.TryGetEntity<ICommandable<T>>(item.EntityId, out var entity);
                entity.Execute(item.Data);
            }
        }

        private readonly struct ApplyMementosAction : IGenericAction<IMementoData>
        {
            private readonly TimelineService service;
            private readonly GenericCollection mementos;

            public ApplyMementosAction(TimelineService service, GenericCollection mementos)
            {
                this.service = service;
                this.mementos = mementos;
            }

            public void Invoke<T>() where T : IMementoData
            {
                service.ApplyMementos<T>(mementos);
            }
        }

        private readonly struct SaveMementosAction : IGenericAction<IMementoData>
        {
            private readonly TimelineService service;
            private readonly GenericCollection mementos;

            public SaveMementosAction(TimelineService service, GenericCollection mementos)
            {
                this.service = service;
                this.mementos = mementos;
            }

            public void Invoke<T>() where T : IMementoData
            {
                service.SaveMementos<T>(mementos);
            }
        }

        private readonly struct ApplyCommandAction : IGenericAction<ICommandData>
        {
            private readonly TimelineService service;
            private readonly GenericCollection commands;

            public ApplyCommandAction(TimelineService service, GenericCollection commands)
            {
                this.service = service;
                this.commands = commands;
            }

            public readonly void Invoke<T>() where T : ICommandData
            {
                service.ApplyCommand<T>(commands);
            }
        }

    }
}
