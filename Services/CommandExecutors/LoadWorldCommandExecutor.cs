using Arch.Core;
using DVG.Core;
using DVG.SkyPirates.Shared.Commands;
using DVG.SkyPirates.Shared.IServices;

namespace DVG.SkyPirates.Shared.Services.CommandExecutors
{
    public class LoadWorldCommandExecutor :
        ICommandExecutor<LoadWorldCommand>
    {
        private readonly World _world;
        private readonly ITimelineService _timelineService;

        public LoadWorldCommandExecutor(World world, ITimelineService timelineService)
        {
            _timelineService = timelineService;
            _world = world;
        }

        public void Execute(Command<LoadWorldCommand> cmd)
        {
            cmd.Data.WorldData.Apply(_world);
            _timelineService.CurrentTick = cmd.Tick;
        }
    }
}