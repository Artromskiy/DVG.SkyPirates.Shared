using DVG.Core;

namespace DVG.SkyPirates.Shared.IServices
{
    public interface ITimelineService
    {
        int CurrentTick { get; }
        void AddCommand<T>(Command<T> command) where T: ICommandData;
        void RemoveCommand(int clientId, int commandId);
        void Tick(float deltaTime);
    }
}
