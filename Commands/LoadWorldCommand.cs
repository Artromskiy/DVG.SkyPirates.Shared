using DVG.Core.Commands.Attributes;
using DVG.SkyPirates.Shared.Data;
using System.Runtime.Serialization;

namespace DVG.SkyPirates.Shared.Commands
{
    [Command]
    public partial struct LoadWorldCommand
    {
        [DataMember(Order = 0)]
        public WorldData WorldData { get; set; }
        [DataMember(Order = 1)]
        public CommandsData CommandsData { get; set; }
        [DataMember(Order = 2)]
        public int CurrentTick { get; set; }

        public LoadWorldCommand(WorldData worldData, CommandsData commandsData, int currentTick)
        {
            WorldData = worldData;
            CommandsData = commandsData;
            CurrentTick = currentTick;
        }
    }
}
