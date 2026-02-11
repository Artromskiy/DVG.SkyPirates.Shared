using DVG.Commands;
using DVG.Core;
using System;

namespace DVG.SkyPirates.Shared.IServices
{
    public interface ICommandRecieveService
    {
        void RegisterReciever<T>(Action<Command<T>> reciever) where T : ICommandData;
        void UnregisterReciever<T>(Action<Command<T>> reciever) where T : ICommandData;
        void InvokeCommand<T>(Command<T> command) where T : ICommandData;
    }
}
