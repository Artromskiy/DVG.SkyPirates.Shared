namespace DVG.SkyPirates.Shared.Commands.Lifecycle
{
    public readonly struct DespawnUnit
    {
        public readonly int unitId;

        public DespawnUnit(int unitId)
        {
            this.unitId = unitId;
        }
    }
}
