using DVG.Components;
using DVG.Core.Commands.Attributes;
using DVG.SkyPirates.Shared.Data;
using DVG.SkyPirates.Shared.Ids;
using System.Runtime.Serialization;

namespace DVG.SkyPirates.Shared.Commands
{
    [Command]
    [DataContract]
    public partial struct SpawnUnitCommand
    {
        public SyncId SquadId;

        public UnitId UnitId;
        public EntityParameters CreationData;
    }
}
