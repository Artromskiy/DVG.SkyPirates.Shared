using DVG.Core;
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

        public LoadWorldCommand(WorldData worldData)
        {
            WorldData = worldData;
        }
    }
}
