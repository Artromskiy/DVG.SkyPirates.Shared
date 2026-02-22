namespace DVG.SkyPirates.Shared.IServices
{
    public interface ICommandExecutorService
    {
        void Tick(int tick, fix deltaTime);
    }
}
