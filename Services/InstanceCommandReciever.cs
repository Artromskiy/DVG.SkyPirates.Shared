using DVG.Core;
using DVG.SkyPirates.Shared.Commands;
using DVG.SkyPirates.Shared.IServices;

namespace DVG.SkyPirates.Shared.Services
{
    public class InstanceCommandReciever
    {
        private readonly IInstanceIdsService _instanceIdsService;
        private readonly ICommandRecieveService _commandService;

        public InstanceCommandReciever(ICommandRecieveService commandService, IInstanceIdsService instanceIdsService)
        {
            _commandService = commandService;
            _instanceIdsService = instanceIdsService;

            _commandService.RegisterReciever<Position>(TryInvokeCommand);
            _commandService.RegisterReciever<Rotation>(TryInvokeCommand);
            _commandService.RegisterReciever<Fixation>(TryInvokeCommand);
        }

        public void TryInvokeCommand<T>(Command<T> cmd)
            where T : unmanaged, ICommandData
        {
            if (_instanceIdsService.TryGetInstance<ICommandReciever<T>>(cmd.EntityId, out var instance))
                instance.Recieve(cmd.Data);
        }
    }
}
