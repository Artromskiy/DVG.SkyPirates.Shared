namespace DVG.SkyPirates.Shared.IServices.TickableExecutors
{

    public interface ITickable
    {
        void Tick(int tick);
    }

    public interface ITickableExecutor :
        IPreTickable,
        IPostTickable,
        ITickable
    { }

    public interface IPreTickable : ITickable { }
    public interface IPostTickable : ITickable { }
}
