using System;

namespace DVG.SkyPirates.Server.IServices
{
    public interface IMessageRecieveService
    {
        public void RegisterReciever<T>(Action<T, int> reciever);
        public void UnregisterReciever<T>(Action<T, int> reciever);
    }
}
