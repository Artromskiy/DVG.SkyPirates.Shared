using DVG.Components;
using DVG.Core.Commands.Attributes;
using System.Runtime.Serialization;

namespace DVG.SkyPirates.Shared.Commands
{
    [Command]
    [DataContract]
    public partial struct SpawnSquadCommand
    {
        public RandomSeed RandomSeed;
        public SyncIdReserve SyncIdReserve;
    }
}
