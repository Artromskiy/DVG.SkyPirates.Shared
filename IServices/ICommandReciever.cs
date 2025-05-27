namespace DVG.SkyPirates.Shared.IServices
{
    public interface ICommandReciever<T>
    {
        void Recieve(T cmd);
    }
}
