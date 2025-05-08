namespace DVG.SkyPirates.Server.IServices
{
    public interface IMessageSendService
    {
        public void SendToAll<T>(T data) where T : unmanaged;
        public void SendTo<T>(T data, int clientId) where T : unmanaged;
    }
}
