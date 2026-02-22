using DVG.Core.Commands.Attributes;
using DVG.SkyPirates.Shared.Data;

namespace DVG.SkyPirates.Shared.Commands
{
    [Command]
    public partial struct LoadWorldCommand
    {
        public WorldData WorldData { get; set; }

        public LoadWorldCommand(WorldData worldData)
        {
            WorldData = worldData;
        }
    }
}
