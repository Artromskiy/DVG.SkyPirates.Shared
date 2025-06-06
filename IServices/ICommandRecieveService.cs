using DVG.Core;
using System;

namespace DVG.SkyPirates.Shared.IServices
{
    public interface ICommandRecieveService
    {
        public void RegisterReciever<T>(Action<Command<T>> reciever) where T : ICommandData;
        public void UnregisterReciever<T>(Action<Command<T>> reciever) where T : ICommandData;
    }
}
