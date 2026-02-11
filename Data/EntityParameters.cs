using DVG.Components;

namespace DVG.SkyPirates.Shared.Data
{
    public readonly struct EntityParameters
    {
        public readonly SyncId SyncId;
        public readonly SyncIdReserve SyncIdReserve;
        public readonly RandomSource RandomSource;

        public EntityParameters(SyncId syncId, SyncIdReserve syncIdReserve, RandomSource randomSource)
        {
            SyncId = syncId;
            SyncIdReserve = syncIdReserve;
            RandomSource = randomSource;
        }
    }
}
