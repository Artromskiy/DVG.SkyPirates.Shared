using DVG.Core.Commands.Attributes;
using DVG.SkyPirates.Shared.Data;

namespace DVG.SkyPirates.Shared.Commands
{
    [Command]
    public partial struct SpawnSquadCommand
    {
        public EntityParameters CreationData;
    }
}
