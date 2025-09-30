using Arch.Core;
using DVG.Core;
using DVG.SkyPirates.Shared.Commands;
using DVG.SkyPirates.Shared.Data;
using DVG.SkyPirates.Shared.IServices;

namespace DVG.SkyPirates.Shared.Services.CommandExecutors
{
    public class LoadWorldCommandExecutor :
        ICommandExecutor<TimelineStartCommand>
    {
        private readonly World _world;
        private readonly ITimelineService _timelineService;

        public LoadWorldCommandExecutor(World world, ITimelineService timelineService)
        {
            _timelineService = timelineService;
            _world = world;
        }

        public void Execute(Command<TimelineStartCommand> cmd)
        {
            WorldDataSerializer.Deserialize(_world, cmd.Data.WorldData);
            _timelineService.CurrentTick = cmd.Tick;
        }
    }
}