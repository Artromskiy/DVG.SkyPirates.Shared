using DVG.Commands.Attributes;
using DVG.SkyPirates.Shared.Data;

namespace DVG.SkyPirates.Shared.Commands
{
    [Command(false)]
    public struct LoadWorldCommand
    {
        public WorldData WorldData;
    }
}
