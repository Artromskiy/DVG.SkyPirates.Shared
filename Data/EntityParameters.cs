using DVG.Components;

namespace DVG.SkyPirates.Shared.Data
{
    public readonly struct EntityParameters
    {
        public readonly SyncId SyncId;
        public readonly SyncIdReserve SyncIdReserve;
        public readonly RandomSeed RandomSeed;

        public EntityParameters(SyncId syncId, SyncIdReserve syncIdReserve, RandomSeed randomSeed)
        {
            SyncId = syncId;
            SyncIdReserve = syncIdReserve;
            RandomSeed = randomSeed;
        }
    }
}
