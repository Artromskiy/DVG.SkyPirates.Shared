namespace DVG.SkyPirates.Shared.IServices.TickableExecutors
{
    public interface ITickableExecutor :
        IPreTickable,
        IPostTickable,
        ITickable
    { }

    public interface ITickable
    {
        void Tick(int tick);
    }

    public interface IPreTickable : ITickable { }
    public interface IPostTickable : ITickable { }
}
