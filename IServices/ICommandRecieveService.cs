using DVG.Commands;
using System;

namespace DVG.SkyPirates.Shared.IServices
{
    public interface ICommandRecieveService
    {
        void RegisterReciever<T>(Action<Command<T>> reciever);
        void UnregisterReciever<T>(Action<Command<T>> reciever);
        void InvokeCommand<T>(Command<T> command);
    }
}
