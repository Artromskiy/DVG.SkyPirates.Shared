using DVG.Commands;

namespace DVG.SkyPirates.Shared.IServices
{
    public interface ICommandExecutor : IGenericCaller
    {
        void Execute<T>(Command<T> cmd);
        void IGenericCaller.Call<A>(ref A action) { }
    }

    public interface ICommandExecutor<K> : ICommandExecutor
    {
        void Execute(Command<K> cmd);
        void ICommandExecutor.Execute<T>(Command<T> cmd) { }

        void IGenericCaller.Call<A>(ref A action)
        {
            action.Invoke<K>();
        }
    }
}
