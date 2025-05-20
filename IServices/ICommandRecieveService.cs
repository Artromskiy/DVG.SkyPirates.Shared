using DVG.SkyPirates.Shared.Commands;
using System;

namespace DVG.SkyPirates.Shared.IServices
{
    public interface ICommandRecieveService
    {
        public void RegisterReciever<T>(Action<Command<T>, int> reciever) where T : unmanaged;
        public void UnregisterReciever<T>(Action<Command<T>, int> reciever) where T : unmanaged;
    }
}
