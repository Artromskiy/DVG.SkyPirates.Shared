using DVG.Core.Commands.Attributes;
using DVG.SkyPirates.Shared.Data;

namespace DVG.SkyPirates.Shared.Commands
{
    [Command]
    public partial struct LoadWorldCommand
    {
        public WorldData WorldData { get; set; }
        public CommandsData CommandsData { get; set; }
        public int CurrentTick { get; set; }

        public LoadWorldCommand(WorldData worldData, CommandsData commandsData, int currentTick)
        {
            WorldData = worldData;
            CommandsData = commandsData;
            CurrentTick = currentTick;
        }
    }
}
