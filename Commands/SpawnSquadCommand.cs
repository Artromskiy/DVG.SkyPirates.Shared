using DVG.Core.Commands.Attributes;
using DVG.SkyPirates.Shared.Data;
using System.Runtime.Serialization;

namespace DVG.SkyPirates.Shared.Commands
{
    [Command]
    [DataContract]
    public partial struct SpawnSquadCommand
    {
        public EntityParameters CreationData;
    }
}
