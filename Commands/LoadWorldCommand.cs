using DVG.Core;
using DVG.Core.Commands.Attributes;
using DVG.SkyPirates.Shared.Data;
using System.Runtime.Serialization;

namespace DVG.SkyPirates.Shared.Commands
{
    [Command]
    public partial struct ConnectionCommand
    {

        [DataMember(Order = 0)]
        public WorldData WorldData { get; set; }
        [DataMember(Order = 1)]
        public CommandsData CommandsData { get; set; }

        public ConnectionCommand(WorldData worldData, CommandsData commandsData)
        {
            WorldData = worldData;
            CommandsData = commandsData;
        }
    }
}
