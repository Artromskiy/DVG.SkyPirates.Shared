﻿using DVG.Core;
using DVG.SkyPirates.Shared.Commands;
using DVG.SkyPirates.Shared.IServices;

namespace DVG.SkyPirates.Shared.Services
{
    public class InstanceCommandReciever
    {
        private readonly IEntitiesService _instanceIdsService;
        private readonly ICommandRecieveService _commandService;

        public InstanceCommandReciever(ICommandRecieveService commandService, IEntitiesService instanceIdsService)
        {
            _commandService = commandService;
            _instanceIdsService = instanceIdsService;

            _commandService.RegisterReciever<Position>(TryInvokeCommand);
            _commandService.RegisterReciever<Rotation>(TryInvokeCommand);
            _commandService.RegisterReciever<Fixation>(TryInvokeCommand);
        }

        public void TryInvokeCommand<T>(Command<T> cmd)
            where T : ICommandData
        {
            if (_instanceIdsService.TryGetEntity<ICommandable<T>>(cmd.EntityId, out var instance))
                instance.Execute(cmd.Data);
        }
    }
}
