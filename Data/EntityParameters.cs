using DVG.Components;

namespace DVG.SkyPirates.Shared.Data
{
    public readonly struct EntityParameters
    {
        public readonly SyncId SyncId;
        public readonly SyncIdReserve SyncIdReserve;
        public readonly RandomSeed RandomSource;

        public EntityParameters(SyncId syncId, SyncIdReserve syncIdReserve, RandomSeed randomSource)
        {
            SyncId = syncId;
            SyncIdReserve = syncIdReserve;
            RandomSource = randomSource;
        }
    }
}
