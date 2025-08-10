#region Reals
using real = System.Single;
using real2 = DVG.float2;
using real3 = DVG.float3;
using real4 = DVG.float4;
#endregion

using DVG.Core;

namespace DVG.SkyPirates.Shared.IServices
{
    public interface ITimelineService
    {
        int CurrentTick { get; }
        real TickTime { get; set; }
        void AddCommand<T>(Command<T> command) where T: ICommandData;
        void RemoveCommand(int clientId, int commandId);
        void Tick();
    }
}
