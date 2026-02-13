using DVG.Components;
using DVG.Core.Commands.Attributes;
using DVG.SkyPirates.Shared.Ids;
using System.Runtime.Serialization;

namespace DVG.SkyPirates.Shared.Commands
{
    [Command]
    [DataContract]
    public partial struct SpawnUnitCommand
    {
        public UnitId UnitId;
        public SyncId SquadId;
        public RandomSeed RandomSeed;
        public SyncIdReserve SyncIdReserve;
    }
}
